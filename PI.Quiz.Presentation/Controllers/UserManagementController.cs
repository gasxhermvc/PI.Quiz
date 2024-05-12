using Microsoft.AspNetCore.Mvc;
using PI.Quiz.Bussiness.Core;
using PI.Quiz.Bussiness.Models.Requests.UserManagement;
using PI.Quiz.DAL.Entities;
using PI.Quiz.Engine.Authentication.Jwt;
using PI.Quiz.Presentation.Security.Middlewares.Authorization;
using System.Net;

namespace PI.Quiz.Presentation.Controllers
{
    [AuthorizeWithPolicy("Admin")]
    [Route("api/um")]
    [ApiController]
    public class UserManagementController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IJwtService<UserInfo> _jwt;
        public UserManagementController(AppDbContext appDbContext
            , IJwtService<UserInfo> jwt)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        }

        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] UserManagementListRequest request)
        {
            try
            {
                var results = new List<object>();
                using (var context = _appDbContext)
                {
                    var queryUsers = context.Users.Where((UmUser user) =>
                        user.Role != "SuperAdmin" && user.Deleted == false).AsQueryable();

                    //=>ค้นหาข้อมูลที่ตรงกับ Keyword
                    if (!string.IsNullOrEmpty(request.Keyword))
                    {
                        var _keyword = request.Keyword.ToLower().Trim();

                        queryUsers = queryUsers.Where((UmUser user) =>
                            (user.FirstName.ToLower().Trim().Contains(_keyword) || user.LastName.ToLower().Trim().Contains(_keyword) || user.NickName.ToLower().Trim().Contains(_keyword))).AsQueryable();
                    }

                    var users = queryUsers.ToList();

                    results = (from u in users
                               select new
                               {
                                   id = u.Username,
                                   firstName = u.FirstName,
                                   lastName = u.LastName,
                                   email = u.Email,
                                   birthDate = u.BirthDate,
                                   phoneNumber = u.PhoneNumber,
                                   role = u.Role,
                                   createdDate = u.CreatedDate,
                                   updatedDate = u.UpdatedDate
                               }).ToList<object>();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, new
                {
                    Message = "Get users successfully.",
                    Data = results
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

        [HttpPut("{Username}/update")]
        public async Task<IActionResult> Update(string Username, [FromForm] UserManagementUpdateRequest request)
        {
            try
            {
                var currentUser = _jwt.User;

                using (var context = _appDbContext)
                {
                    var user = context.Users.FirstOrDefault((UmUser u) =>
                        u.Username == Username && u.Deleted == false && u.Activated == true);

                    if (user is null)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, $"The username dosen't exists."));
                    }

                    //=>สิทธิ์ที่ส่งมาไม่ตรงจะเช็ทให้เป็น User เป็น Default
                    if (request.Role != "Admin" && request.Role != "User")
                    {
                        request.Role = "User";
                    }

                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;
                    user.NickName = request.NickName;
                    user.BirthDate = request.BirthDate;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Activated = request.Activated;
                    user.UpdatedDate = DateTime.Now;
                    user.Role = request.Role;

                    context.SaveChanges();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, new
                {
                    Message = "Update user successfully."
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

        [HttpDelete("{Username}/delete")]
        public async Task<IActionResult> Delete(string Username)
        {
            try
            {
                using (var context = _appDbContext)
                {
                    var user = context.Users.FirstOrDefault((UmUser u) =>
                        u.Username == Username && u.Deleted == false);

                    if (user is null)
                    {
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.Conflict, $"The username dosen't exists."));
                    }

                    user.Deleted = true;
                    user.UpdatedDate = DateTime.Now;

                    context.SaveChanges();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, new
                {
                    Message = "Deleted user successfully."
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
