using Application.Configurations.Extensions.Help;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace HouseFinancesAPI.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerAuthorizationJWT(this IServiceCollection services)
        {

            services.AddSwaggerGen(swagger =>
            {
                swagger.SchemaFilter<DateSchemaFilter>();

                swagger.SwaggerDoc("v1", new OpenApiInfo
                { 
                    Title = "House Finances API", 
                    Version = "v1" 
                });

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

        public static void ConfigureSwaggerUI(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "House Finances API");
                
                c.RoutePrefix = "Doc";
                c.DocumentTitle = "House Finances API";
                c.HeadContent = app.ReadFileFromRootPath("html/swagger-head.html");
            });
        }

    }
}
