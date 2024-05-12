using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
