using Application.Configurations.Extensions.DependencyManagers;
using Application.Configurations.UserMain;
using Data.Configurations.Extensions;
using HouseFinancesAPI.Extensios.Swagger;
using HouseFinancesAPI.FiltersControllers;
using ProEventos.API.Configuration.Middleware;
using System.Text.Json;

namespace HouseFinancesAPI
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(LogInformationFilter));
            }).AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            services.AddEndpointsApiExplorer();
            services.AddConectionsString(Configuration);
            services.AddApiDependencyServices(Configuration);
            services.AddSwaggerAuthorizationJWT();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment env)
        {
            app.ConfigureSwaggerUI(env);

            app.UseCorsPolicy();

            app.UseHttpsRedirection();

            app.UseRouting();

            services.ConfigurarBancoDados();

            app.UseMiddleware<MiddlewareException>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                options.MapGet("/{*path}", async context =>
                {
                    context.Response.Redirect("/house-finances");
                    await Task.CompletedTask;
                });
            });
        }
    }
}