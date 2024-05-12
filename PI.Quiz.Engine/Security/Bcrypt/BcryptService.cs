using Microsoft.Extensions.Configuration;

namespace PI.Quiz.Engine.Security.Bcrypt
{
    public class BcryptService : IBcryptService
    {
        private readonly IConfiguration _configuration;

        private string _saltingPassword { get; set; } = string.Empty;

        public BcryptService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _saltingPassword = _configuration["WebServiceSettings:Crypt:SaltingPassword"] ?? throw new ArgumentNullException(nameof(_saltingPassword));
        }

        public bool Check(string input, string hashing)
        {
            return BC.Verify(input, hashing);
        }

        public string Hash(string input)
        {
            return BC.HashPassword(input, _saltingPassword);
        }
    }
}
