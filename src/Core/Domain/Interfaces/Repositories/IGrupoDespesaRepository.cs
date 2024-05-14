using Domain.Interfaces.Repositories.Base;
using Domain.Models.Despesas;

namespace Domain.Interfaces.Repositories
{
    public interface IGrupoDespesaRepository : IRepositoryBase<GrupoDespesa>
    {
        Task<GrupoDespesa> ExisteAsync(int id = 0, string nome = null);
    }
}
