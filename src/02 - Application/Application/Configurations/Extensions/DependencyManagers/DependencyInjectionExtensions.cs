using Application.Interfaces.Services;
using Application.Interfaces.Utility;
using Application.Services.Usuario;
using Application.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Application.Services.LogApp;
using Data.Repository.LogApp;
using Domain.Interfaces.Repository.LogApp;
using Domain.Interfaces.Repository.Finance;
using Application.Services.Finance;
using Data.Repository.Finance;

namespace Application.Configurations.Extensions.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<ILogApplicationRepository, LogApplicationRepository>();
            services.AddScoped<IFinanceRepository, FinanceRepository>();
        }
        public static void AddDependecyServices(this IServiceCollection services)
        {
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<ILogApplicationServices, LogApplicationServices>();
            services.AddScoped<IFinanceServices, FinanceServices>();
        }
    }
}
