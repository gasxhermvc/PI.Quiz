using Microsoft.Extensions.DependencyInjection;

namespace PI.Quiz.Engine.Security.Crypto
{
    public static class CryptoServiceRegisterExtension
    {
        /// <summary>
        /// อินเจกต์การใช้งานเข้ารหัส
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCryptoService(this IServiceCollection services)
        {
            services.AddSingleton<ICryptoService, CryptoService>();

            return services;
        }
    }
}
