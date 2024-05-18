using Data.DataContext;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using Infraestructure.Data.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Repository.Membros
{
    public class MembroRepository(IServiceProvider service)
        : RepositoryBase<Membro, FinanceDbContext>(service),
            IMembroRepository
    {
        public async Task<Membro> ExisteAsync(string nome) =>
            await Get(d => d.Nome == nome).FirstOrDefaultAsync();

        public bool ValidaMembroParaAcao(int idMembro)
        {
            var (idJhon, idPeu) = GetIdsJhonPeu();

            var ehAlteravel = idMembro == idJhon || idMembro == idPeu;

            return ehAlteravel;
        }

        public (int, int) GetIdsJhonPeu()
        {
            var membros = Get();

            int idJhon = membros.FirstOrDefault(c => c.Nome.StartsWith("Jhon")).Id;
            int idPeu = membros.FirstOrDefault(c => c.Nome.StartsWith("Peu")).Id;

            return (idJhon, idPeu);
        }
    }
}
