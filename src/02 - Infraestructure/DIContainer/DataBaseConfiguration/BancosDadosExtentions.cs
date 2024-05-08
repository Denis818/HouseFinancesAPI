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
                    new MySqlServerVersion(new Version(8, 4, 0)),
                    mySqlOptions => mySqlOptions
                        .EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null
                        )
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
