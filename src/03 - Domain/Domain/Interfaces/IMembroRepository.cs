using Domain.Models;

namespace Domain.Interfaces
{
    public interface IMembroRepository : IRepositoryBase<Membro>
    {
        Task<Membro> ExisteAsync(string nome);
        bool ValidaMembroParaAcao(int idMembro);
        (int idJhon, int idPeu) GetIdsJhonPeu();
    }
}
