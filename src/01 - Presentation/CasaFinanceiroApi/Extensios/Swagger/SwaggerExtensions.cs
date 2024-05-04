using Asp.Versioning;
using Asp.Versioning.Conventions;
using CasaFinanceiroApi.Extensios.Swagger.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CasaFinanceiroApi.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                })
                .AddMvc(options =>
                {
                    options.Conventions.Add(new VersionByNamespaceConvention());
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo() { Title = "Casa Financeiro v1", Version = "v1" }
                );

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.CustomSchemaIds(x => x.FullName);

                AddSecuritySchema(options);

                options.SchemaFilter<DateSchemaFilter>();
                options.OperationFilter<GrupoDespesaIdHeaderParameter>();

            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        public static void AddSecuritySchema(SwaggerGenOptions options)
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

        public static void ConfigureSwaggerUI(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "API v1");

                options.RoutePrefix = "doc";
                options.DocumentTitle = "Casa Financeiro API";
                options.HeadContent = File.ReadAllText(
                    Path.Combine(app.Environment.WebRootPath, "html/swagger-head.html")
                );
            });
        }
    }
}
