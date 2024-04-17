using Application.Utilities;
using Data.DataContext;
using Domain.Dtos.User;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Finance;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public static class SeedUser
    {
        public static void ConfigurarBancoDados(this IServiceProvider serviceProvider)
        {
            using var serviceScope = serviceProvider.CreateScope();
            var services = serviceScope.ServiceProvider;

            var vendasDbContext = services.GetRequiredService<FinanceDbContext>();
            if(!vendasDbContext.Database.CanConnect())
            {
                vendasDbContext.Database.Migrate();
                PrepareCategoryAndMember(services).Wait();
            }

            PrepareUserAdmin(services);
        }

        public static void PrepareUserAdmin(IServiceProvider serviceProvider)
        {
            var usuarioRepository = serviceProvider.GetRequiredService<IUsuarioRepository>();

            string email = "master@gmail.com";
            string senha = "Master@123456";

            if(usuarioRepository.Get(u => u.Email == email).FirstOrDefault() != null)
                return;

            var (Salt, PasswordHash) = new PasswordHasher().CriarHashSenha(senha);

            var usuario = new Usuario
            {
                Email = email,
                Password = PasswordHash,
                Salt = Salt,
                //Permissoes = []
            };

            var permissoes = new EnumPermissoes[]
            {
                EnumPermissoes.USU_000001,
                EnumPermissoes.USU_000002,
                EnumPermissoes.USU_000003,
            };

            usuarioRepository.InsertAsync(usuario).Wait();
            usuarioRepository
                .AddPermissaoAsync(new AddUserPermissionDto(usuario.Id, permissoes))
                .Wait();
        }

        public static async Task PrepareCategoryAndMember(IServiceProvider service)
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

            await categoriaRepository.InsertRangeAsync(listCategoria);
            await memberRepository.InsertRangeAsync(listMember);
        }
    }
}
