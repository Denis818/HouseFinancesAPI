using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Presentation.Base;
using Presentation.Configurations.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Presentation.Configurations.Extensions
{
    public static class ApiConfig
    {
        public const string V1 = "1.0";
        public const string V2 = "2.0";

        public static void ConfigureWebApi(this IServiceCollection services)
        {
            var presentationAssembly = typeof(MainController).Assembly;

            services
                .AddControllers(options =>
                {
                    options.Conventions.Add(new ApiVersioningFilter());
                })
                .AddApplicationPart(presentationAssembly)
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services
                .AddApiVersioning(setup =>
                {
                    setup.AssumeDefaultVersionWhenUnspecified = true;
                    setup.ReportApiVersions = true;
                    setup.DefaultApiVersion = new ApiVersion(1, 0);
                })
                .AddApiExplorer(options =>
                {
                    options.SubstituteApiVersionInUrl = true;
                    options.GroupNameFormat = "'v'VVV";
                });
        }

        public static void VersioningApiGenSwagger(this SwaggerGenOptions swagger)
        {
            swagger.SwaggerDoc(
                "v1.0",
                new OpenApiInfo() { Title = "API v1", Version = "v1.0" }
            );
            swagger.SwaggerDoc(
                "v2.0",
                new OpenApiInfo() { Title = "API v2", Version = "v2.0" }
            );
        }

        public static void VersioningApiUISwagger(this SwaggerUIOptions swagger)
        {
            swagger.SwaggerEndpoint($"/swagger/v1.0/swagger.json", "API v1");
            swagger.SwaggerEndpoint($"/swagger/v2.0/swagger.json", "API v2");
        }
    }
}
