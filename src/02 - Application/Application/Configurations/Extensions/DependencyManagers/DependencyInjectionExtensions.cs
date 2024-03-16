using Application.Interfaces.Services;
using Application.Interfaces.Utility;
using Application.Services.Usuario;
using Application.Utilities;
using Data.Repository;
using Domain.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;
using Application.Services.LogApp;

namespace Application.Configurations.Extensions.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<ILogApplicationRepository, LogApplicationRepository>();
        }
        public static void AddDependecyServices(this IServiceCollection services)
        {
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<ILogApplicationServices, LogApplicationServices>();
        }
    }
}
