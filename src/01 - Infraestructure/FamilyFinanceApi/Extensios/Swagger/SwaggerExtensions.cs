using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace FamilyFinanceApi.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerAuthorizationJWT(this IServiceCollection services)
        {
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "FamilyFinanceApi", Version = "v1" });
                swagger.SchemaFilter<DateSchemaFilter>();

                swagger.ExampleFilters();

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Auth Bearer Scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                swagger.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new [] {"Bearer"}}
                };

                swagger.AddSecurityRequirement(securityRequirement);
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }
    }

    public class DateSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
            {
                schema.Example = new OpenApiString(DateTime.Now.ToString("dd-MM-yyyy"));
            }
        }
    }
}
