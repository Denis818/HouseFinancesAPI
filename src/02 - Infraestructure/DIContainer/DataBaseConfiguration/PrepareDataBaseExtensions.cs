using System.Globalization;
using Application.Helpers;
using Application.Interfaces.Services.User;
using Domain.Converters.DatesTimes;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Domain.Models.Users;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration
{
    public class PrepareDataBaseExtensions
    {
        public static void PrepareDataBase(IServiceProvider service, string nomeDominio)
        {
            PrepareUserMaster(service, nomeDominio);
            PrepararVisitante(service);
            PrepareCategoryAndMember(service).Wait();
        }

        private static void PrepareUserMaster(IServiceProvider service, string nomeDominio)
        {
            var usuarioRepository = service.GetRequiredService<IUsuarioRepository>();
            var authService = service.GetRequiredService<IAuthAppService>();

            string email = "master@gmail.com";
            string senha = "Master@123456";

            if (nomeDominio.Contains("dev") || nomeDominio.Contains("railway"))
            {
                email = "dev@gmail.com";
                senha = "dev@123";
            }

            if (usuarioRepository.Get(u => u.Email == email).FirstOrDefault() != null)
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
            usuarioRepository.SaveChangesAsync().Wait();

            authService.AddPermissaoAsync(new AddUserPermissionDto(usuario.Id, permissoes)).Wait();
        }

        private static void PrepararVisitante(IServiceProvider service)
        {
            var usuarioRepository = service.GetRequiredService<IUsuarioRepository>();
            var authService = service.GetRequiredService<IAuthAppService>();

            string email = "visitante";
            string senha = "123456";

            if (usuarioRepository.Get(u => u.Email == email).FirstOrDefault() != null)
                return;

            var (Salt, PasswordHash) = new PasswordHasherHelper().CriarHashSenha(senha);

            var usuario = new Usuario
            {
                Email = email,
                Password = PasswordHash,
                Salt = Salt,
                Permissoes = []
            };

            usuarioRepository.InsertAsync(usuario).Wait();
            usuarioRepository.SaveChangesAsync().Wait();
        }

        private static async Task PrepareCategoryAndMember(IServiceProvider service)
        {
            var categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            var memberRepository = service.GetRequiredService<IMembroRepository>();
            var grupoDespesaRepository = service.GetRequiredService<IGrupoDespesaRepository>();

            if (categoriaRepository.Get().ToList().Count > 0)
                return;

            var listCategoria = new List<Categoria>
            {
                new() { Descricao = "Almoço/Janta" },
                new() { Descricao = "Condomínio" },
                new() { Descricao = "Aluguel" },
                new() { Descricao = "Limpeza" },
                new() { Descricao = "Lanches" },
                new() { Descricao = "Higiêne" },
                new() { Descricao = "Internet" },
                new() { Descricao = "Conta de Luz" }
            };

            var listMember = new List<Membro>
            {
                new() { Nome = "Bruno", Telefone = "(38) 99805-5965" },
                new() { Nome = "Denis", Telefone = "(38) 997282407" },
                new() { Nome = "Valdirene", Telefone = "(31) 99797-7731" },
                new() { Nome = "Peu", Telefone = "(38) 99995-4309" },
                new() { Nome = "Jhon Lenon", Telefone = "(31) 99566-4815" }
            };

            string mesAtualName = DateTimeZoneProvider
                .GetBrasiliaTimeZone(DateTime.UtcNow)
                .ToString("MMMM", new CultureInfo("pt-BR"));
            mesAtualName = char.ToUpper(mesAtualName[0]) + mesAtualName[1..].ToLower();

            var grupoDespesa = new GrupoDespesa
            {
                Nome = $"Fatura de {mesAtualName} {DateTime.Now.Year}"
            };

            await categoriaRepository.InsertRangeAsync(listCategoria);
            await memberRepository.InsertRangeAsync(listMember);
            await grupoDespesaRepository.InsertAsync(grupoDespesa);

            await categoriaRepository.SaveChangesAsync();
            await memberRepository.SaveChangesAsync();
            await grupoDespesaRepository.SaveChangesAsync();
        }
    }
}
