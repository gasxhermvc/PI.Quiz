using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PI.Quiz.Engine.Authentication.Jwt
{
    public class JWTService<TUser> : IJwtService<TUser>
        where TUser : class
    {
        protected IConfiguration _configuration { get; set; }

        protected IHttpContextAccessor _context { get; set; }

        public override bool IsAuthenticated { get => this.Authentication(); }

        public override TUser User { get; }

        public JWTService(IConfiguration configuration, IHttpContextAccessor context)
        {
            _configuration = configuration;
            _context = context;
        }

        public override string GetRawAccessToken()
        {
            StringValues accessToken = string.Empty;
            _context.HttpContext.Request.Headers.TryGetValue("Authorization", out accessToken);

            return accessToken.ToString().Replace("Bearer ", string.Empty) ?? string.Empty;
        }

        public override Dictionary<string, dynamic> GenerateToken(List<Claim> claims)
        {
            //=>default = 5 min
            var timeout = int.Parse(_configuration.GetSection("WebServiceSettings:OAuth:AccessTokenExpires")?.Value ?? "300");

            DateTime expired = DateTime.Now.AddSeconds(timeout);

            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("WebServiceSettings:OAuth:SecretKey").Value));

            SigningCredentials creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _configuration.GetSection("WebServiceSettings:OAuth:Issuer").Value,
                Expires = expired,
                SigningCredentials = creds,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return new Dictionary<string, dynamic>{
                { "AccessToken", tokenHandler.WriteToken(token) },
                { "TokenType", "bearer" },
                { "ExpiresIn", timeout },
                { "RefreshToken", Guid.NewGuid().ToString("N") }
            };
        }

        public override ClaimsPrincipal UserContext()
        {
            return _context.HttpContext.User;
        }

        private bool Authentication()
        {
            var userContext = this.UserContext();

            if (userContext.Claims != null && userContext.Claims.Count() > 0)
            {
                var identifier = userContext.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier);

                var username = userContext.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name).Value;

                return identifier != null && !string.IsNullOrEmpty(username);
            }

            return false;
        }
    }
}
