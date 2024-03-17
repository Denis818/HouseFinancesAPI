using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace FamilyFinanceApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hosting, loggerConfiguration) =>
                {
                    loggerConfiguration
                    .ReadFrom.Configuration(hosting.Configuration).Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm} {Level:u3}] {Message:lj}{NewLine}{Exception}\n",
                     theme: AnsiConsoleTheme.Sixteen);
                })
                .ConfigureAppConfiguration((hosting, config) =>
                {
                    config.AddJsonFile($"appsettings.{hosting.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
                    webBuilder.UseStartup<Startup>();
                             // .UseUrls($"http://0.0.0.0:{port}");
                });
    }
}