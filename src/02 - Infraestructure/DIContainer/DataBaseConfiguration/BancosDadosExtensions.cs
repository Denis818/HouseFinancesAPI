using Data.Configurations;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public static class SeedUser
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionStrings = config
                .GetSection("CompanyConnectionStrings:List")
                .Get<List<CompanyInfo>>();

            foreach(var company in connectionStrings)
            {
                services.AddDbContext<FinanceDbContext>(options =>
                    options.UseMySql(
                        company.ConnnectionString,
                        ServerVersion.AutoDetect(company.ConnnectionString)
                    )
                );

                using var scope = services.BuildServiceProvider().CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

                dbContext.Database.SetConnectionString(company.ConnnectionString);
                dbContext.Database.Migrate();

                PrepareDataBaseExtensions.PrepareDataBase(
                    scope.ServiceProvider,
                    company.NomeDominio
                );
            }
        }
    }
}
