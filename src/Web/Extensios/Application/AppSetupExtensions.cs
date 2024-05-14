using Presentation.Version;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Web.Extensios.DependencyManagers;
using Web.Extensios.Swagger;

namespace Web.Extensios.Application
{
    public static class AppSetupExtensions
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            string env = builder.Environment.EnvironmentName;
            string port = Environment.GetEnvironmentVariable("PORT") ?? "3000";

            builder.Host.UseSerilog(
                (hosting, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hosting.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(
                            outputTemplate: "[{Timestamp:HH:mm} {Level:u3}] {Message:lj}{NewLine}{Exception}\n",
                            theme: AnsiConsoleTheme.Literate
                        );
                }
            );

            builder.Configuration.AddJsonFile(
                $"appsettings.{env}.json",
                optional: false,
                reloadOnChange: true
            );

            builder.Services.ConfigureWebApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiDependencyServices(builder.Configuration);
            builder.Services.AddSwaggerConfiguration();
#if(PRD)
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#endif
        }
    }
}
