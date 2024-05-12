using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Text;
using PI.Quiz.Engine.Security.Bcrypt;
using PI.Quiz.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using PI.Quiz.Engine.Security.Crypto;
using PI.Quiz.Engine.Authentication.Jwt;
using PI.Quiz.Bussiness.Core;
using PI.Quiz.Presentation.Security.Middlewares.Excpetion;
using PI.Quiz.Presentation.Security.Middlewares.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace PI.Quiz.Presentation;

public class Startup
{
    public IConfiguration Configuration { get; }

    private readonly string _locale = "en-US";
    private readonly string _allowSpecificOrigins = "_GasxherMvcWebServiceAllowSpecificOrigins";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        IdentityModelEventSource.ShowPII = true;

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {

            options.UseSqlite($"Data Source=app.db")
                   .UseInternalServiceProvider(serviceProvider);
        }).AddEntityFrameworkSqlite();

        // If using Kestrel:
        services.Configure<KestrelServerOptions>((KestrelServerOptions options) =>
        {
            options.AllowSynchronousIO = true;
            options.AddServerHeader = false;

            //=>If you want to secure request, required to set it.
            //=>Example: Limit about 30 MB. per request, 1024 * 1024 * 30;
            options.Limits.MaxRequestBodySize = null;
        });

        // If using IIS:
        services.Configure<IISServerOptions>((IISServerOptions options) =>
        {
            options.AllowSynchronousIO = true;

            //=>If you want to secure request, required to set it.
            //=>Example: Limit about 30 MB. per request, 1024 * 1024 * 30;
            options.MaxRequestBodySize = null;
        });

        services.AddCors(options =>
        {
            options.AddPolicy(_allowSpecificOrigins, builder =>
            {
                Configuration.GetSection("WebServiceSettings:CorsPolicy")
                .GetChildren()
                .ToList()
                .ForEach((IConfigurationSection corsPolicy) =>
                {
                    builder.WithOrigins(corsPolicy?.Value ?? "*")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
        });


        services.AddControllersWithViews().AddNewtonsoftJson(options =>
        {
            //=>Set response json
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.Culture = new CultureInfo(_locale);
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        });

        //=>Register services in app
        services.AddHttpContextAccessor();

        //=>Add Anti-CSRF token middleware
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN"; // ชื่อ header ที่ใช้สำหรับส่ง token ไปยัง server
        });

        //=>Hander permissions
        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdmin", policy =>
                policy.Requirements.Add(new AuthorizeWithRolesRequirement("SuperAdmin")));

            options.AddPolicy("Admin", policy =>
                policy.Requirements.Add(new AuthorizeWithRolesRequirement("SuperAdmin", "Admin")));

            options.AddPolicy("User", policy =>
                policy.Requirements.Add(new AuthorizeWithRolesRequirement("SuperAdmin", "Admin", "User")));
        });
        services.AddSingleton<IAuthorizationHandler, RoleHandler>();

        // OAuth2: Resource Server
        services.AddAuthentication((AuthenticationOptions options) =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer((JwtBearerOptions options) =>
        {
            string secretKey = Configuration.GetValue<string>("WebServiceSettings:OAuth:SecretKey") ?? string.Empty;
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new NullReferenceException(nameof(secretKey));
            }

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.Audience = Configuration.GetValue<string>("WebServiceSettings:OAuth:Issuer");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidAudience = Configuration.GetValue<string>("WebServiceSettings:OAuth:Issuer"),
                ValidIssuer = Configuration.GetValue<string>("WebServiceSettings:OAuth:Issuer"),
                ClockSkew = TimeSpan.Zero
            };
        });

        //=>Register customize services
        services.AddBcryptService();
        services.AddCryptoService();
        services.AddJwtService(options =>
        {
            options.AddScoped(typeof(IJwtService<UserInfo>), typeof(UserContext<UserInfo>));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //=>Setup locale all app
        var local = new List<CultureInfo>() { new CultureInfo(_locale) };
        app.UseRequestLocalization((options) =>
        {
            options.DefaultRequestCulture = new RequestCulture(_locale);
            options.SupportedCultures = local;
            options.SupportedUICultures = local;
            options.RequestCultureProviders.Clear();
        });


        //=>Customize basepath from IIS
        string basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH") ?? string.Empty;
        if (string.IsNullOrEmpty(basePath))
        {
            //=>Default base path
            app.UsePathBase("/pi-quiz");
        }
        else
        {
            app.Use(async (context, next) =>
            {
                context.Request.PathBase = basePath;
                await next.Invoke();
            });
        }

        //=>Dev mode
        if (env.IsDevelopment())
        {
            //=>Display error pages
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandlingMiddleware();

        //=>Prod mode
        if (env.IsProduction())
        {
            //=>Use https transportation only
            app.UseHsts();

            //=>Auto redirect to https
            app.UseHttpsRedirection();
        }

        //=>CORS middleware 
        app.UseCors(_allowSpecificOrigins);

        //=>Use routing middleware
        app.UseStaticFiles();
        app.UseRouting();


        //=>Authen middleware
        app.UseAuthentication();
        app.UseAuthorization();

        //=>Use controller route
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}

