using Application.Helpers;
using Application.Interfaces.Services.User;
using Data.DataContext;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Domain.Models.Membros;
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

            var dbContext = services.GetRequiredService<FinanceDbContext>();

            dbContext.Database.Migrate();

            PrepareUserAdmin(services);

            PrepareCategoryAndMember(services).Wait();
        }

        public static void PrepareUserAdmin(IServiceProvider services)
        {
            var usuarioRepository = services.GetRequiredService<IUsuarioRepository>();
            var authService = services.GetRequiredService<IAuthAppService>();

            string email = "master@gmail.com";
            string senha = "Master@123456";

            if(usuarioRepository.Get(u => u.Email == email).FirstOrDefault() != null)
                return;

            var (Salt, PasswordHash) = new PasswordHasherHelper().CriarHashSenha(senha);

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
            authService.AddPermissaoAsync(new AddUserPermissionDto(usuario.Id, permissoes)).Wait();
        }

        public static async Task PrepareCategoryAndMember(IServiceProvider service)
        {
            var categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            var memberRepository = service.GetRequiredService<IMembroRepository>();

            if(categoriaRepository.Get().ToList().Count > 0) return;

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
                new() { Nome = "Bruno", Telefone = "(38) 9 9805-5965" },
                new() { Nome = "Denis", Telefone = "(38) 9 97282407" },
                new() { Nome = "Valdirene", Telefone = "(31) 9 9797-7731" },
                new() { Nome = "Peu", Telefone = "(38) 9 9995-4309" },
                new() { Nome = "Jhon Lenon", Telefone = "(31) 9 9566-4815" }
            };

            await categoriaRepository.InsertRangeAsync(listCategoria);
            await memberRepository.InsertRangeAsync(listMember);

            await categoriaRepository.SaveChangesAsync();
            await memberRepository.SaveChangesAsync();
        }
    }
}
