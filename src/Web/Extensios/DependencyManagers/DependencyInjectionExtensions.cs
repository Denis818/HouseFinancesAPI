﻿using Application.Interfaces.Services.Categorias;
using Application.Interfaces.Services.Despesas;
using Application.Interfaces.Services.Membros;
using Application.Interfaces.Services.User;
using Application.Interfaces.Utilities;
using Application.Services;
using Application.Services.Categorias;
using Application.Services.Despesas.Operacoes;
using Application.Services.Despesas.ProcessamentoDespesas;
using Application.Services.GrupoFaturas;
using Application.Services.Membros;
using Application.Services.User;
using Application.Utilities;
using Data.Repository.Categorias;
using Data.Repository.Despesas;
using Data.Repository.User;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
using Domain.Services;
using Infraestructure.Data.Configurations;
using Infraestructure.Data.Repository.Membros;
using Presentation.ModelState;
using Presentation.ModelState.Interface;
using Web.Middleware;

namespace Web.Extensios.DependencyManagers
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependecyUtilities(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IMapper, Mapper>();
            services.AddScoped<INotifier, Notifier>();
            services.AddScoped<IModelStateValidator, ModelStateValidator>();
        }

        public static void AddDependecyRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IGrupoFaturaRepository, GrupoFaturaRepository>();
            services.AddScoped<IStatusFaturaRepository, StatusFaturaRepository>();
        }

        public static void AddDependecyDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IDespesaDomainServices, DespesaDomainServices>();
        }

        public static void AddDependecyAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthAppService, AuthAppService>();
            services.AddScoped<IMembroAppServices, MembroAppServices>();
            services.AddScoped<ICategoriaAppServices, CategoriaAppServices>();
            services.AddScoped<IDespesaAppService, DespesaAppService>();
            services.AddScoped<IDespesaConsultas, DespesaConsultas>();

            services.AddScoped<IDespesaCrudAppService, DespesaCrudAppService>();
            services.AddScoped<IDespesaCasaAppService, DespesaCasaAppService>();
            services.AddScoped<IDespesaMoradiaAppService, DespesaMoradiaAppService>();
            services.AddScoped<IGrupoFaturaAppService, GrupoFaturaAppService>();
            services.AddScoped<IStatusFaturaAppService, StatusFaturaAppService>();
        }

        public static void AddCompanyConnectionStrings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var appSettingsSection = configuration.GetSection(nameof(CompanyConnectionStrings));
            var appSettings = appSettingsSection.Get<CompanyConnectionStrings>();
            services.AddSingleton(appSettings);
        }

        public static void AddDependecyMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<ExceptionMiddleware>();
            services.AddTransient<IdentificadorDataBaseMiddleware>();
        }
    }
}
