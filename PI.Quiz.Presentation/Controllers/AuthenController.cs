using Microsoft.AspNetCore.Mvc;
using PI.Quiz.Bussiness.Core;
using PI.Quiz.Bussiness.Models.Requests.Authen;
using PI.Quiz.DAL.Entities;
using PI.Quiz.Engine.Authentication.Jwt;
using PI.Quiz.Engine.Security.Bcrypt;
using PI.Quiz.Presentation.Security.Middlewares.Authorization;
using System.Net;
using System.Security.Claims;

namespace PI.Quiz.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;
        private readonly IJwtService<UserInfo> _jwt;
        private readonly IBcryptService _bcrypt;

        public AuthenController(IConfiguration configuration
            , AppDbContext appDbContext
            , IJwtService<UserInfo> jwt
            , IBcryptService bcrypt)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); ;
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
            _bcrypt = bcrypt ?? throw new ArgumentNullException(nameof(bcrypt));
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromForm] AuthenTokenRequest request)
        {
            try
            {
                using (var context = _appDbContext)
                {
                    switch (request.GrantType)
                    {
                        case "password":
                            return await Task.FromResult(Ok(this.CreateAccessToken(request, context)));
                        case "refresh_token":
                            return await Task.FromResult(Ok(this.RefreshAccessToken(request, context)));
                        default:
                            throw new UnauthorizedAccessException("The grantType provide \'password\' or \'refresh_token\' only.");
                    }
                }
            }
            catch (BadHttpRequestException rex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, new
                {
                    message = rex.Message
                }));
            }
            catch (UnauthorizedAccessException uaex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.Unauthorized, new
                {
                    message = uaex.Message
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.ServiceUnavailable, new
                {
                    message = ex.Message
                }));
            }
        }

        [AuthorizeWithPolicy("User")]
        [HttpGet("user-info")]
        public async Task<IActionResult> UserInfo()
        {
            try
            {
                UserInfo userInfo = _jwt.User;

                return await Task.FromResult(Ok(new
                {
                    Message = "Get user info successfully",
                    Data = userInfo
                }));
            }
            catch (UnauthorizedAccessException uaex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.Unauthorized, new
                {
                    Message = uaex.Message
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

        [AuthorizeWithPolicy("User")]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                var rawToken = _jwt.GetRawAccessToken();

                using (var context = _appDbContext)
                {
                    var token = context.Tokens.FirstOrDefault((UmToken ut) =>
                        ut.AccessToken == rawToken && ut.Deleted == false);

                    if (token is null)
                    {
                        throw new UnauthorizedAccessException($"The token dosen't exists.");
                    }

                    if (token.Revoked)
                    {
                        throw new UnauthorizedAccessException($"The token has been revoked.");
                    }

                    if (token.ExpiredDate < DateTime.Now)
                    {
                        throw new UnauthorizedAccessException($"The token has been expired.");
                    }

                    token.Revoked = true;
                    context.SaveChanges();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, new
                {
                    message = "Logged out successfully."
                }));
            }
            catch (UnauthorizedAccessException uaex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.Unauthorized, new
                {
                    Messaeg = uaex.Message
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

        #region Helpers
        private Dictionary<string, object> CreateAccessToken(AuthenTokenRequest request, AppDbContext context)
        {
            var user = _appDbContext.Users.FirstOrDefault((UmUser u) =>
                                u.Username == request.Username && u.Password == _bcrypt.Hash(request.Password) && u.Deleted == false);

            if (user is null)
            {
                throw new UnauthorizedAccessException("The username or password incorrect.");
            }

            if (!user.Activated)
            {
                throw new UnauthorizedAccessException("The username not been activated.");
            }

            List<Claim> claims = new List<Claim>
                            {
                                new Claim("username", user.Username),
                                new Claim("email", user.Email),
                                new Claim("firstName",user.FirstName),
                                new Claim("lastName",user.LastName),
                                new Claim("nickName",user.NickName),
                                new Claim("phoneNumber", user.PhoneNumber),
                                new Claim("role", user.Role),
                                new Claim("nonce", Guid.NewGuid().ToString("N"))
                            };

            var token = _jwt.GenerateToken(claims);
            var now = DateTime.Now.AddSeconds(token["ExpiresIn"]);

            context.Tokens.Add(new UmToken
            {
                AccessToken = token["AccessToken"],
                RefreshToken = token["RefreshToken"],
                Revoked = false,
                ExpiredIn = token["ExpiresIn"],
                ExpiredDate = now,
                Deleted = false,
                CreatedDate = now,
                User = user,
            });

            context.SaveChanges();

            token["message"] = "Generate access token successfully.";

            return token;
        }

        private Dictionary<string, object> RefreshAccessToken(AuthenTokenRequest request, AppDbContext context)
        {
            var currentUser = _jwt.User;

            var accessToken = context.Tokens.FirstOrDefault((UmToken ut) =>
                                ut.AccessToken == _jwt.GetRawAccessToken() && ut.Deleted == false);

            if (accessToken is null)
            {
                throw new UnauthorizedAccessException("The accessToken dosen't exists.");
            }

            if (accessToken.RefreshToken != request.RefreshToken)
            {
                throw new UnauthorizedAccessException("The refeshToken not mateches.");
            }

            if (accessToken.Revoked)
            {
                throw new UnauthorizedAccessException("The accessToken has been revoked.");
            }

            if (accessToken.ExpiredDate < DateTime.Now)
            {
                throw new UnauthorizedAccessException("The accessToken has been expired.");
            }

            accessToken.Revoked = true;
            accessToken.UpdatedDate = DateTime.Now;

            var user = context.Users.FirstOrDefault((UmUser u) =>
                            u.Username == currentUser.Username && u.Deleted == false);

            if (user is null)
            {
                throw new UnauthorizedAccessException("The username or password incorrect.");
            }

            if (!user.Activated)
            {
                throw new UnauthorizedAccessException("The username not been activated.");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim("username", user.Username),
                new Claim("email", user.Email),
                new Claim("firstName",user.FirstName),
                new Claim("lastName",user.LastName),
                new Claim("nickName",user.NickName),
                new Claim("phoneNumber", user.PhoneNumber),
                new Claim("role", user.Role),
                new Claim("nonce", Guid.NewGuid().ToString("N"))
            };

            var token = _jwt.GenerateToken(claims);
            var now = DateTime.Now.AddSeconds(token["ExpiresIn"]);

            context.Tokens.Add(new UmToken
            {
                AccessToken = token["AccessToken"],
                RefreshToken = token["RefreshToken"],
                Revoked = false,
                ExpiredIn = token["ExpiresIn"],
                ExpiredDate = now,
                Deleted = false,
                CreatedDate = now,
                User = user,
            });

            context.SaveChanges();

            token["message"] = "Refresh token successfully.";

            return token;
        }
        #endregion
    }
}
