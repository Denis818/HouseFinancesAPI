using Domain.Dtos.Categoria;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Finance;

namespace Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        Task<Categoria> ExisteAsync(int id = 0, string nome = null);
        bool ValidaCategoriaParaAcao(int idCategoria);
        CategoriaIdsDto GetCategoriaIds();
    }
}
