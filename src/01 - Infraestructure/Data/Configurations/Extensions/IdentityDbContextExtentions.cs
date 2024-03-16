using Data.DataContext;
using Data.DataContext.Context;
using Domain.Dtos.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Configurations.Extensions
{
    public static class IdentityDbContextExtentions
    {
        public static void AddConectionsString(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionVendas = configuration.GetConnectionString("FINANCE");
            string connectionLog = configuration.GetConnectionString("APPLICATIONLOG");

            services.AddDbContext<FinanceDbContext>(options =>
             options.UseMySql(connectionVendas, ServerVersion.AutoDetect(connectionVendas)));

            services.AddDbContext<LogDbContext>(options =>
             options.UseMySql(connectionLog, ServerVersion.AutoDetect(connectionLog)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<FinanceDbContext>()
                    .AddDefaultTokenProviders();
        }
    }
}
