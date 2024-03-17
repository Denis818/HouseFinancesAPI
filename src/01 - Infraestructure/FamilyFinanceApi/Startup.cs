using Application.Configurations.Extensions.DependencyManagers;
using Application.Configurations.UserMain;
using Data.Configurations.Extensions;
using FamilyFinanceApi.Extensios.Swagger;
using FamilyFinanceApi.FiltersControllers;
using ProEventos.API.Configuration.Middleware;

namespace FamilyFinanceApi
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(LogInformationFilter));
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddConectionsString(Configuration);
            services.AddApiDependencyServices(Configuration);
            services.AddSwaggerAuthorizationJWT();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider services)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

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
                    context.Response.Redirect("/swagger");
                    await Task.CompletedTask;
                });
            });
        }
    }
}