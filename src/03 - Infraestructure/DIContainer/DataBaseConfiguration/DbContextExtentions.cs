using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public static class DbContextExtentions
    {
        public static void AddConectionsString(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            string connectionVendas = configuration.GetConnectionString("FINANCE");

            services.AddDbContext<FinanceDbContext>(options =>
                options.UseMySql(connectionVendas, ServerVersion.AutoDetect(connectionVendas))
            );
        }
    }
}
