using CasaFinanceiroApi.ControllerHelpers;
using CasaFinanceiroApi.Extensios.Swagger;
using DIContainer.DependencyManagers;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;

namespace CasaFinanceiroApi.Extensios.Application
{
    public static class AppSetupExtensions
    {
        public static void SetupApplication(this WebApplicationBuilder builder)
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


            builder
                .Services.AddControllers(options =>
                {
                    options.Conventions.Add(new ApiVersioningFilter());
                })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiDependencyServices(builder.Configuration);
            builder.Services.AddSwaggerConfiguration();
#if(PRD)
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#endif
        }
    }
}
