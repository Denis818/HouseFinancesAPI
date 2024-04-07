using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces;
using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Finance
{
    public class CategoriaRepository(IServiceProvider service) : 
        RepositoryBase<Categoria, FinanceDbContext>(service), ICategoriaRepository
    {
        public async Task<Categoria> ExisteAsync(int id, string nome)
        {
            if (nome != null)
            {
                return await Get(c => c.Descricao == nome).FirstOrDefaultAsync();

            }
            else
            {
                return await Get(c => c.Id == id).FirstOrDefaultAsync();
            }
        }

        public bool ValidaCategoriaParaAcao(int idCategoria)
        {
            var (idAlmoco, idAluguel) = GetIdsAluguelAlmoco();

            var ehAlteravel = idCategoria == idAlmoco || idCategoria == idAluguel;

            return ehAlteravel;
        }

        public (int, int) GetIdsAluguelAlmoco()
        {
            var categorias = Get();

            int idAlmoco = categorias.FirstOrDefault(c => c.Descricao.StartsWith("Almoço")).Id;
            int idAluguel = categorias.FirstOrDefault(c => c.Descricao.StartsWith("Aluguel")).Id;

            return (idAlmoco, idAluguel);
        }
    }
}
