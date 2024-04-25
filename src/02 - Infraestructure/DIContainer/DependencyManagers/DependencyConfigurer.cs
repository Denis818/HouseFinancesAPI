﻿using Application.Validators.Despesas;
using DIContainer.DataBaseConfiguration;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace DIContainer.DependencyManagers
{
    public static class DependencyConfigurer
    {
        public static void AddApiDependencyServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddConectionsString(config);

            services.AddDependecyUtilities();
            services.AddDependecyRepositories();
            services.AddDependecyAppServices();

            services.AddAuthenticationJwt(config);
            services.AddAssemblyConfigurations();
            services.AddLocalization(options => options.ResourcesPath = "03 - Core/Application/Resources/Messages");
        }

        public static void AddAuthenticationJwt(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            var key = Encoding.ASCII.GetBytes(config["Jwt:key"]);
            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = config["TokenConfiguration:Issuer"],
                        ValidAudience = config["TokenConfiguration:Audience"]
                    };
                });
        }

        public static void AddAssemblyConfigurations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(DespesaValidator)));
        }

        public static void UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }

        public static void UseLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[] { "pt-BR" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                                                                      .AddSupportedCultures(supportedCultures)
                                                                      .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

        }
    }
}
