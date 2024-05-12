using System.Security.Claims;

namespace PI.Quiz.Engine.Authentication.Jwt
{
    public abstract class IJwtService<TUser>
          where TUser : class
    {
        public abstract string GetRawAccessToken();
        public abstract Dictionary<string, dynamic> GenerateToken(List<Claim> claims);
        public abstract ClaimsPrincipal UserContext();
        public abstract bool IsAuthenticated { get; }
        public abstract TUser User { get; }
    }
}
