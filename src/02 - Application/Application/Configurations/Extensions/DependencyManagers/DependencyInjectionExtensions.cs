using Application.Interfaces.Services;
using Application.Interfaces.Utility;
using Application.Services.Usuario;
using Application.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Application.Services.LogApp;
using Data.Repository.LogApp;
using Application.Services.Finance;
using Data.Repository.Finance;
using Domain.Interfaces;
using Application.Interfaces.Services.Finance;
using Application.Interfaces.Services.LogApp;
using Application.Interfaces.Services.User;

namespace Application.Configurations.Extensions.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<ILogApplicationRepository, LogApplicationRepository>();
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        }
        public static void AddDependecyServices(this IServiceCollection services)
        {
            services.AddScoped<IUserServices, UserServices>();

            services.AddScoped<ILogApplicationServices, LogApplicationServices>();
            services.AddScoped<IDespesaServices, DespesaServices>();
            services.AddScoped<IMembroServices, MembroServices>();
            services.AddScoped<ICategoriaServices, CategoriaServices>();
        }
    }
}
