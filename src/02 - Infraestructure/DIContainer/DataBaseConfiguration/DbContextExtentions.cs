using Data.Configurations;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public static class DbContextExtentions
    {
        public static void AddConnectionStrings(this IServiceCollection services, IConfiguration configuration)
        {
            var companyConnections = configuration.GetSection("CompanyConnectionStrings").Get<CompanyConnectionStrings>();
            foreach(var company in companyConnections.List)
            {
                services.AddDbContext<FinanceDbContext>(options =>
                    options.UseMySql(company.ConnnectionString, ServerVersion.AutoDetect(company.ConnnectionString)),
                    contextLifetime: ServiceLifetime.Transient, // Importante para evitar o compartilhamento de contexto
                    optionsLifetime: ServiceLifetime.Transient
                );
            }
        }

    }
}
