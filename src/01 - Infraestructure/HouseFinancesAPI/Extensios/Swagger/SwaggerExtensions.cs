using Application.Configurations.Extensions.Help;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.OpenApi.Models;

namespace HouseFinancesAPI.Extensios.Swagger
{
    public static class SwaggerExtensions
    {
        const string TypeToken = "Bearer";

        public static void AddSwaggerAuthorizationJWT(this IServiceCollection services)
        {

            services.AddSwaggerGen(swagger =>
            {
                swagger.SchemaFilter<DateSchemaFilter>();

                //swagger.SwaggerDoc("v1", new OpenApiInfo
                //{ 
                //    Title = "House Finances API", 
                //    Version = "v1" 
                //});

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = $"JWT Auth {TypeToken} Scheme",
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

                swagger.AddSecurityDefinition(TypeToken, securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new [] {TypeToken}}
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
                c.HeadContent = File.ReadAllText(
                    Path.Combine(app.Environment.WebRootPath,"html/swagger-head.html"));
            });
        }

    }
}
