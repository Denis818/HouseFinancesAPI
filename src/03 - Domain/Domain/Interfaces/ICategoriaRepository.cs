using Domain.Models;

namespace Domain.Interfaces
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        Task<Categoria> ExisteAsync(int id = 0, string nome = null);
        bool ValidaCategoriaParaAcao(int idCategoria);
        (int idAlmoco, int idAluguel) GetIdsAluguelAlmoco();
    }
}
