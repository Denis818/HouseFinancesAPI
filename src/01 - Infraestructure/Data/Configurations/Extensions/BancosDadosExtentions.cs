using Data.DataContext;
using Data.DataContext.Context;
using Domain.Dtos.User;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Finance;
using Domain.Models.Users;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Configurations.Extensions
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
        }

        public static void PrepareUserAdmin(IServiceProvider serviceProvider)
        {
            var usuarioRepository = serviceProvider.GetRequiredService<IUsuarioRepository>();

            string email = "master@gmail.com";
            string senha = "Master@123456";

            if (usuarioRepository.Get(u => u.Email == email)
                                 .FirstOrDefault() != null) return;

            var generetaHash = new PasswordHasher();
            var (Salt, PasswordHash) = generetaHash.CriarHashSenha(senha);

            var usuario = new Usuario
            {
                Email = email,
                Password = PasswordHash,
                Salt = Salt,
                Permissoes = []
            };

            var permissoes = new EnumPermissoes[]
            {
                 EnumPermissoes.USU_000001,
                 EnumPermissoes.USU_000002,
                 EnumPermissoes.USU_000003,
            };

            usuarioRepository.InsertAsync(usuario).Wait();
            usuarioRepository.AddPermissaoAsync(new AddUserPermissionDto(usuario.Id, permissoes)).Wait();
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
