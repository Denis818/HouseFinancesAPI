using Application.Interfaces.Services;
using Application.Interfaces.Utility;
using Application.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Application.Services.LogApp;
using Data.Repository.LogApp;
using Application.Services.Finance;
using Data.Repository.Finance;
using Domain.Interfaces;
using Application.Interfaces.Services.Finance;
using Application.Interfaces.Services.LogApp;
using Application.Services.User;
using Data.Repository.User;
using Application.Interfaces.Services.User;
using Domain.Utilities;

namespace Application.Configurations.Extensions.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyUtilities(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<PasswordHasher>();
            services.AddScoped<INotificador, Notificador>();
        }

        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ILogApplicationRepository, LogApplicationRepository>();
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        }
        public static void AddDependecyServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ILogApplicationServices, LogApplicationServices>();
            services.AddScoped<IDespesaServices, DespesaServices>();
            services.AddScoped<IMembroServices, MembroServices>();
            services.AddScoped<ICategoriaServices, CategoriaServices>();
        }
    }
}
