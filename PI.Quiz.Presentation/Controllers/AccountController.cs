using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PI.Quiz.Bussiness.Models.Requests.Account;
using PI.Quiz.DAL.Entities;
using PI.Quiz.Engine.Security.Bcrypt;
using PI.Quiz.Engine.Security.Crypto;
using System.Net;

namespace PI.Quiz.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IBcryptService _bcrypt;
        private readonly ICryptoService _crypto;

        public AccountController(AppDbContext appDbContext
            , IBcryptService bcrypt
            , ICryptoService crypt)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _bcrypt = bcrypt ?? throw new ArgumentNullException(nameof(bcrypt));
            _crypto = crypt ?? throw new ArgumentNullException(nameof(crypt));
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken] //=>ควรจะมีการ Custom เป็นพิเศษเพื่อให้ได้ Error Response ที่กลมกลืนกับส่วนอื่น ๆ
        public async Task<IActionResult> Register([FromForm] AccountRegisterRequest request)
        {
            try
            {
                var now = DateTime.Now;

                //=>Mapper model
                var user = new UmUser()
                {
                    Username = request.Email,
                    Password = _bcrypt.Hash(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    NickName = request.NickName,
                    BirthDate = request.BirthDate,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = "User",
                    Activated = true,
                    Deleted = false,
                    CreatedDate = now,
                };

                //=>Add and saved
                using (var context = _appDbContext)
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.Created, new
                {
                    Message = "Created user successfully."
                }));
            }
            catch (BadHttpRequestException rex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, new
                {
                    Message = rex.Message
                }));
            }
            catch (DbUpdateException duex)
            {
                if (duex != null && duex.InnerException is not null && duex.InnerException is SqliteException)
                {
                    SqliteException sqlEx = duex.InnerException as SqliteException ?? throw new NullReferenceException(nameof(duex.InnerException));
                    switch (sqlEx.SqliteErrorCode)
                    {
                        case 19: //=>ข้อมูลในระบบซ้ำ หรือตรงกับ Unique key
                            return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, new
                            {
                                Message = sqlEx.Message
                            }));
                    }
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.UnprocessableEntity, new
                {
                    Message = duex.Message
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.ServiceUnavailable, new
                {
                    Message = ex.Message
                }));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] AccountForgotPasswordRequest request)
        {
            try
            {
                using (var context = _appDbContext)
                {
                    //=>Find user with email
                    var user = context.Users.FirstOrDefault((UmUser u) =>
                        u.Email == request.Email && u.Deleted == false);
                    if (user is null)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, $"The email \'{request.Email}\' dosen't exists."));
                    }

                    //=>Life time of token is 10 minutes.
                    var now = DateTime.Now;
                    var expiredDate = DateTime.Now.AddMinutes(10);
                    var duration = (expiredDate - now);
                    var payload = new
                    {
                        ExpiredDate = expiredDate,
                        Username = user.Username,
                        Password = user.Password
                    };

                    //=>ในขั้นตอนนี้ควรจะมี Trasaction คุมด้วยเพื่อให้แน่ใจว่าสามารถบันทึกข้อมูลสำเร็จทั้ง UmResetPassword และการส่ง Email
                    var token = _crypto.Encrypt(JsonConvert.SerializeObject(payload, Formatting.None));
                    context.ResetPasswords.Add(new UmResetPassword
                    {
                        Token = token,
                        ExpiredDate = expiredDate,
                        ExpiredIn = (long)duration.TotalSeconds,
                        User = user,
                        Deleted = false,
                        Revoked = false,
                        CreatedDate = DateTime.Now
                    });
                    context.SaveChanges();
                    //=>Send toke to email.
                    //=>Your process to send email.

                    //=>เป็นการจำลองเท่านั้น ในสถานการณ์จริง Token ที่สร้างได้ต้องส่งผ่าน Email เท่านั้นประการนี้ทำเพื่อเอา Token ไปยืนยันต่อในขั้นตอนเปลี่ยนรหัสผ่าน
                    return await Task.FromResult(Ok(new
                    {
                        Message = "Generate token for reset password successfully",
                        Token = token
                    }));
                }
            }
            catch (BadHttpRequestException rex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, new
                {
                    Message = rex.Message
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.ServiceUnavailable, new
                {
                    Message = ex.Message
                }));
            }
        }

        [HttpPatch("reset-password/{Token}")]
        public async Task<IActionResult> ResetPassword(string Token, [FromForm] AccountResetPasswordRequest request)
        {
            try
            {
                using (var context = _appDbContext)
                {
                    //=>Find token from reset password table
                    var token = context.ResetPasswords.FirstOrDefault((UmResetPassword rp) =>
                        rp.Token == Token && rp.Deleted == false);
                    if (token is null)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, new
                        {
                            Message = $"The token \'{Token}\' dosen't exists."
                        }));
                    }

                    if (token.Revoked)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, new
                        {
                            Message = $"The token is revoke."
                        }));
                    }

                    var now = DateTime.Now;
                    var plainText = _crypto.Decrypt(Token);
                    var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(plainText) ?? throw new Exception("Cannot decrypt data.");

                    if (payload.ContainsKey("ExpiredDate") && ((DateTime)payload["ExpiredDate"]) < now)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, new
                        {
                            Message = "The token is expired."
                        }));
                    }

                    var username = payload["Username"].ToString();
                    var password = payload["Password"].ToString();

                    var user = context.Users.FirstOrDefault((UmUser u) => u.Username == username && u.Password == password);
                    if (user is null)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, new
                        {
                            Message = $"The username dosen't exists."
                        }));
                    }

                    //=>Update passwrod from user table
                    user.Password = _bcrypt.Hash(request.newPassword);

                    //=>Update revoke from reset password table
                    token.Revoked = true;
                    token.UpdatedDate = now;
                    context.SaveChanges();
                }

                //=>เป็นการจำลองเท่านั้น ในสถานการณ์จริง Token ที่สร้างได้ต้องส่งผ่าน Email เท่านั้นประการนี้ทำเพื่อเอา Token ไปยืนยันต่อในขั้นตอนเปลี่ยนรหัสผ่าน
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, new
                {
                    Message = "Reset password successfully."
                }));
            }
            catch (BadHttpRequestException rex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, new
                {
                    Message = rex.Message
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.ServiceUnavailable, new
                {
                    Message = ex.Message
                }));
            }
        }
    }
}
