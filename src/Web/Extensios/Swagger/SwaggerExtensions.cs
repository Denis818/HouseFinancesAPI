using CasaFinanceiroApi.Extensios.Swagger.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Presentation.Configurations.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Web.Extensios.Swagger.Filters;

namespace Web.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.VersioningApiGenSwagger();

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.CustomSchemaIds(x => x.FullName);

                options.AddSecuritySchema();

                options.AddSchemaFilters();
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        public static void AddSecuritySchema(this SwaggerGenOptions options)
        {
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = $"Insira o token JWT desta maneira \"Bearer <seu token>\" ",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { "Bearer" } }
            };

            options.AddSecurityRequirement(securityRequirement);
        }

        public static void AddSchemaFilters(this SwaggerGenOptions options)
        {
            options.SchemaFilter<DateSchemaFilter>();
            options.OperationFilter<GrupoDespesaIdHeaderParameter>();
        }

        public static void ConfigureSwaggerUI(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.VersioningApiUISwagger();

                options.RoutePrefix = "doc";
                options.DocumentTitle = "Casa Financeiro API";
                options.HeadContent = File.ReadAllText(
                    Path.Combine(app.Environment.WebRootPath, "html/swagger-head.html")
                );
            });
        }
    }
}
