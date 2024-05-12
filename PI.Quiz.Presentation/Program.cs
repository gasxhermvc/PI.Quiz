using NLog;
using NLog.Web;
using System.Data.SQLite;

namespace PI.Quiz.Presentation;

public class Program
{
    private static string ASPNETCORE_ENVIRONMENT => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile($"appsettings.{ASPNETCORE_ENVIRONMENT}.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables()
       .Build();

    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
        {
            logger.Info("Getting the GasxherMvc Web Service running...");

            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Host terminated unexpectedly {ex}");
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    public static void InitializeSQLiteFileForDevelopmentMode()
    {
        var dbName = "app.db";
        string temp = string.Empty;

        if (ASPNETCORE_ENVIRONMENT == "Development")
        {
            temp = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../");
        }
        else
        {
            temp = AppDomain.CurrentDomain.BaseDirectory;
        }

        var path = System.IO.Path.Combine(temp, dbName);

        //=>สร้างไฟล์ app.db เฉพาะ Mode Development
        if (!System.IO.File.Exists(path) && ASPNETCORE_ENVIRONMENT == "Development")
        {
            SQLiteConnection.CreateFile(path);
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        InitializeSQLiteFileForDevelopmentMode();

        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Sources.Clear();

                config.AddConfiguration(Configuration);

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .UseNLog();
    }
}