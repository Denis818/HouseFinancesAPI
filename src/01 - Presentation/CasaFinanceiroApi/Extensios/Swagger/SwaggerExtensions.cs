using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CasaFinanceiroApi.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        const string TypeToken = "Bearer";

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
                AddSwaggerVersioningApi(options);
                AddSecuritySchema(options);
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        public static void AddSwaggerVersioningApi(SwaggerGenOptions options)
        {
            options.AddApiVersion("v1", "Casa Financeiro v1");
            options.AddApiVersion("v2", "Casa Financeiro v2");
            options.AddApiVersion("v3", "Casa Financeiro v3");

            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.CustomSchemaIds(x => x.FullName);

            options.SchemaFilter<DateSchemaFilter>();
        }

        public static void AddSecuritySchema(SwaggerGenOptions options)
        {
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = $"Insira o token JWT desta maneira \"{TypeToken} <seu token>\" ",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = TypeToken,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = TypeToken
                }
            };

            options.AddSecurityDefinition(TypeToken, securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { TypeToken } }
            };

            options.AddSecurityRequirement(securityRequirement);
        }

        public static void ConfigureSwaggerUI(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "v1");
                options.SwaggerEndpoint($"/swagger/v2/swagger.json", "v2");
                options.SwaggerEndpoint($"/swagger/v3/swagger.json", "v3");
                options.RoutePrefix = "doc";
                options.DocumentTitle = "Casa Financeiro API";
                options.HeadContent = File.ReadAllText(
                    Path.Combine(app.Environment.WebRootPath, "html/swagger-head.html")
                );
            });
        }

        public static void AddApiVersion(
            this SwaggerGenOptions options,
            string version,
            string title
        )
        {
            options.SwaggerDoc(version, new OpenApiInfo() { Title = title, Version = version });
        }
    }
}
