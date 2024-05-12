using Microsoft.Extensions.DependencyInjection;

namespace PI.Quiz.Engine.Security.Bcrypt
{
    public static class BcryptServiceRegisterExtension
    {
        /// <summary>
        /// อินเจกต์การใช้งานเข้ารหัส
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBcryptService(this IServiceCollection services)
        {
            services.AddSingleton<IBcryptService, BcryptService>();

            return services;
        }
    }
}
