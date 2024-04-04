using Data.DataContext;
using Data.DataContext.Context;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Application.Configurations.UserMain
{
    public static class SeedUser
    {
        public static void ConfigurarBancoDados(this IServiceProvider serviceProvider)
        {
            using var serviceScope = serviceProvider.CreateScope();
            var services = serviceScope.ServiceProvider;

            var vendasDbContext = services.GetRequiredService<FinanceDbContext>();
            if (!vendasDbContext.Database.CanConnect())
            {
                vendasDbContext.Database.Migrate();
                PrepareCategoryAndMember(services);
            }

            var logDbContext = services.GetRequiredService<LogDbContext>();
            if (!logDbContext.Database.CanConnect())
            {
                logDbContext.Database.Migrate();
            }

            PrepareUserAdmin(services);
            PrepareUserMember(services);
        }

        public static void PrepareUserAdmin(IServiceProvider service)
        {
            var _userManager = service.GetRequiredService<UserManager<IdentityUser>>();

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
        public static void PrepareUserMember(IServiceProvider service)
        {
            var _userManager = service.GetRequiredService<UserManager<IdentityUser>>();
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

        public static void PrepareCategoryAndMember(IServiceProvider service)
        {
            var categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            var memberRepository = service.GetRequiredService<IMembroRepository>();

            var listCategoria = new List<Categoria>
            {
                new() { Descricao = "Almoço/Janta" },
                new() { Descricao = "Condomínio" },
                new() { Descricao = "Aluguel" },
                new() { Descricao = "Limpeza" },
                new() { Descricao = "Lanches" },
                new() { Descricao = "Higiêne" },
                new() { Descricao = "Internet" },
                new() { Descricao = "Conta de Água" },
                new() { Descricao = "Conta de Luz" }

            };

            var listMember = new List<Membro>
            {
                new() { Nome = "Bruno" },
                new() { Nome = "Denis" },
                new() { Nome = "Valdirene" },
                new() { Nome = "Peu" },
                new() { Nome = "Jhon Lenon" }
            };

            categoriaRepository.InsertRangeAsync(listCategoria).Wait();
            memberRepository.InsertRangeAsync(listMember).Wait();
        }
    }
}
