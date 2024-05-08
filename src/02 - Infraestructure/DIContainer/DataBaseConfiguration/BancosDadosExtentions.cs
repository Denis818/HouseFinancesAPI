using Data.Configurations;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public static class SeedUser
    {
        public static void AddDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<FinanceDbContext>(options =>
                options.UseMySql(

                    new MySqlServerVersion(new Version(8, 0, 21))
                ));
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
