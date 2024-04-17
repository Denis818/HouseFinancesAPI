using Domain.Interfaces.Repositories.Base;
using Domain.Models.Finance;

namespace Domain.Interfaces.Repositories
{
    public interface IMembroRepository : IRepositoryBase<Membro>
    {
        Task<Membro> ExisteAsync(string nome);
        bool ValidaMembroParaAcao(int idMembro);
        (int idJhon, int idPeu) GetIdsJhonPeu();
    }
}
