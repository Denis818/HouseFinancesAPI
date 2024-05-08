using Data.Configurations;
using Data.DataContext;
using DIContainer.DataBaseConfiguration.ConnectionString;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public static class SeedUser
    {
        public static void AddDbContext(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionStringResolver, ConnectionStringResolver>();

            services.AddDbContext<FinanceDbContext>(
                (serviceProvider, dbContextBuilder) =>
                {
                    var connectionStringResolver =
                        serviceProvider.GetRequiredService<IConnectionStringResolver>();
                    string connectionString = connectionStringResolver.IdentificarStringConexao();

                    dbContextBuilder.UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString)
                    );
                }
            );
        }

        public static void ConfigurarBancoDeDados(
            this IServiceProvider serviceProvider,
            CompanyConnectionStrings companies
        )
        {
            foreach(var company in companies.List)
            {
                using var serviceScope = serviceProvider.CreateScope();
                var service = serviceScope.ServiceProvider;
                var dbContext = service.GetRequiredService<FinanceDbContext>();

                dbContext.Database.SetConnectionString(company.ConnnectionString);
                dbContext.Database.Migrate();

                PrepareDataBaseExtentions.PrepareDataBase(service, company.ConnnectionString);
            }
        }
    }
}
