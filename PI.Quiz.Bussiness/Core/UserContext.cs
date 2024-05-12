using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using PI.Quiz.DAL.Entities;
using PI.Quiz.Engine.Authentication.Jwt;
using System.Security.Claims;

namespace PI.Quiz.Bussiness.Core
{
    public class UserContext<TUser> : JWTService<TUser>
       where TUser : UserInfo

    {
        private readonly AppDbContext _appDbContext;

        public UserContext(IConfiguration configuration
            , IHttpContextAccessor context
            , AppDbContext appDbContext) : base(null, null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        private TUser user = null;

        public override TUser User
        {
            get
            {
                this.GetUserInfo();
                return user;
            }
        }

        private void GetUserInfo()
        {

            if (user == null)
            {
                var UserContext = this.UserContext();
                if (UserContext.Claims != null && UserContext.Claims.Count() > 0)
                {
                    string accessToken = this.GetRawAccessToken();
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new UnauthorizedAccessException("The accessToken dosen't exists.");
                    }

                    var token = _appDbContext.Tokens.FirstOrDefault((UmToken ut) =>
                        ut.AccessToken == accessToken && ut.Deleted == false);

                    if (token is null)
                    {
                        throw new UnauthorizedAccessException("The accessToken dosen't exists.");
                    }

                    if (token.Revoked)
                    {
                        throw new UnauthorizedAccessException("The accessToken has been revoked.");
                    }

                    if (token.ExpiredDate < DateTime.Now)
                    {
                        throw new UnauthorizedAccessException("The accessToken has been expired.");
                    }

                    var username = UserContext.Claims?.FirstOrDefault((Claim c) => c.Type == "username")?.Value ?? string.Empty;

                    UmUser user = _appDbContext.Users.FirstOrDefault((UmUser u) =>
                        u.Username == username && u.Deleted == false && u.Activated == true); ;

                    if (user is null)
                    {
                        throw new UnauthorizedAccessException("The accessToken has been expired.");
                    }

                    this.user = new UserInfo
                    {
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        Lastname = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role,
                    } as TUser;
                }
            }
        }
    }
}
