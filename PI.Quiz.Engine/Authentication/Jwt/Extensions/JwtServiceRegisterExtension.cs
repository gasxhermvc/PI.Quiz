using Microsoft.Extensions.DependencyInjection;

namespace PI.Quiz.Engine.Authentication.Jwt
{
    public static class JwtServiceRegisterExtension
    {
        public static IServiceCollection AddJwtService(this IServiceCollection services, Action<IServiceCollection> options)
        {
            options.Invoke(services);

            return services;
        }
    }
}
