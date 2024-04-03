﻿using HouseFinancesAPI.FiltersControllers;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;

namespace HouseFinancesAPI.Extensios.Application
{
    public static class AppSetupExtensions
    {
        public static void SetupApplication(this WebApplicationBuilder builder)
        {
            string env = builder.Environment.EnvironmentName;
            string port = Environment.GetEnvironmentVariable("PORT") ?? "3000";

            builder.Host.UseSerilog((hosting, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hosting.Configuration)
                                   .Enrich.FromLogContext()
                                   .WriteTo.Console(outputTemplate:
                                        "[{Timestamp:HH:mm} {Level:u3}] {Message:lj}{NewLine}{Exception}\n",
                                   theme: AnsiConsoleTheme.Literate);
            });

            builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true);

          //  builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(typeof(LogInformationFilter));

            }).AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }
    }
}
