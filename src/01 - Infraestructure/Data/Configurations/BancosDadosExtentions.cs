using Data.DataContext;
using Data.DataContext.Context;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Application.Configurations.UserMain
{
    public static class SeedUser
    {
        public static void ConfigurarBancoDados(this IServiceProvider services)
        {
            using var serviceScope = services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var vendasDbContext = serviceProvider.GetRequiredService<FinanceDbContext>();

          //  bool deletado = vendasDbContext.Database.EnsureDeleted();

            if (!vendasDbContext.Database.CanConnect())
            {
                vendasDbContext.Database.Migrate();
            }

            var logDbContext = serviceProvider.GetRequiredService<LogDbContext>();
          //  bool dseletado = logDbContext.Database.EnsureDeleted();

            if (!logDbContext.Database.CanConnect())
            {
                logDbContext.Database.Migrate();
            }

            PrepareUserAdmin(serviceScope);
            PrepareUserMember(serviceScope);
        }

        public static void PrepareUserAdmin(IServiceScope serviceScope)
        {
            var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var usuarioInicial = _userManager.FindByEmailAsync("master@gmail.com").GetAwaiter().GetResult();

            if (usuarioInicial is null)
            {
                IdentityUser user = new()
                {
                    UserName = "master@gmail.com",
                    Email = "master@gmail.com",
                    NormalizedUserName = "master@gmail.com",
                    NormalizedEmail = "MASTER@GMAIL.COM",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                IdentityResult result = _userManager.CreateAsync(user, "Master@123456").Result;

                if (result.Succeeded)
                {
                    var listPermissoesPadroesAdmin = new[]
                    {
                        EnumPermissoes.USU_000001,
                        EnumPermissoes.USU_000002
                    };

                    var claims = listPermissoesPadroesAdmin.Select(p =>
                    new Claim(nameof(EnumPermissoes), p.ToString())).ToList();

                    _userManager.AddClaimsAsync(user, claims).Wait();
                }
            }
        }
        public static void PrepareUserMember(IServiceScope serviceScope)
        {
            var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var usuarioInicial = _userManager.FindByEmailAsync("visitante@gmail.com").GetAwaiter().GetResult();

            if (usuarioInicial is null)
            {
                IdentityUser user = new()
                {
                    UserName = "visitante@gmail.com",
                    Email = "visitante@gmail.com",
                    NormalizedUserName = "visitante@gmail.com",
                    NormalizedEmail = "VISITANTE@GMAIL.COM",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                _userManager.CreateAsync(user, "Abc@123456").Wait();    
            }
        }
    }
}
