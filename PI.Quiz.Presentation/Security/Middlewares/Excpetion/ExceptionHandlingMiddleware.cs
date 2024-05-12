using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace PI.Quiz.Presentation.Security.Middlewares.Excpetion
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);
            switch (httpContext.Response.StatusCode)
            {
                case 401:
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        message = "Un authorization.",
                    }), Encoding.UTF8);
                    break;
                case 403:
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        message = "Access denied.",
                    }), Encoding.UTF8);
                    break;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
