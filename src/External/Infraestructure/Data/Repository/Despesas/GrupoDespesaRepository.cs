using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Despesas
{
    public class GrupoDespesaRepository(IServiceProvider service)
        : RepositoryBase<GrupoDespesa, FinanceDbContext>(service),
            IGrupoDespesaRepository
    {
        public async Task<GrupoDespesa> ExisteAsync(int id, string nome)
        {
            GrupoDespesa grupoDespesa = null;
            if (nome != null)
            {
                grupoDespesa = await Get(c => c.Nome == nome).FirstOrDefaultAsync();
            }
            else
            {
                grupoDespesa = await Get(c => c.Id == id).FirstOrDefaultAsync();
            }

            return grupoDespesa;
        }
    }
}
