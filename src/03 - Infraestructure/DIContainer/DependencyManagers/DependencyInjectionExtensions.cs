using Application.Interfaces.Services.Finance;
using Application.Interfaces.Services.LogApp;
using Application.Interfaces.Services.User;
using Application.Interfaces.Utilities;
using Application.Services.Finance;
using Application.Services.LogApp;
using Application.Services.User;
using Application.Utilities;
using Data.Repository.Finance;
using Data.Repository.LogApp;
using Data.Repository.User;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyUtilities(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IMapper, Mapper>();
            services.AddScoped<INotifier, Notifier>();
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
            services.AddScoped<IAuthAppService, AuthAppService>();

            services.AddScoped<ILogAppServices, LogAppServices>();
            services.AddScoped<IDespesaAppServices, DespesaAppServices>();
            services.AddScoped<IMembroAppServices, MembroAppServices>();
            services.AddScoped<ICategoriaAppServices, CategoriaAppServices>();
        }
    }
}
