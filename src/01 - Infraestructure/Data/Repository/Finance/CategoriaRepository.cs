using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces;
using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Finance
{
    public class CategoriaRepository(IServiceProvider service)
        : RepositoryBase<Categoria, FinanceDbContext>(service),
            ICategoriaRepository
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
            var (idAlmoco, idAluguel, idCondominio, idContaDeLuz) = GetIdsAluguelAlmoco();

            var ehAlteravel =
                idCategoria == idAlmoco
                || idCategoria == idAluguel
                || idCategoria == idCondominio
                || idCategoria == idContaDeLuz;

            return ehAlteravel;
        }

        public (int, int, int, int) GetIdsAluguelAlmoco()
        {
            var categ = Get();

            int idAlmoco = categ.FirstOrDefault(c => c.Descricao.StartsWith("Almoço")).Id;

            int idAluguel = categ.FirstOrDefault(c => c.Descricao.StartsWith("Aluguel")).Id;
            int idCondominio = categ.FirstOrDefault(c => c.Descricao.StartsWith("Condomínio")).Id;
            int idContaDeLuz = categ.FirstOrDefault(c => c.Descricao.StartsWith("Conta de Luz")).Id;

            return (idAlmoco, idAluguel, idCondominio, idContaDeLuz);
        }
    }
}
