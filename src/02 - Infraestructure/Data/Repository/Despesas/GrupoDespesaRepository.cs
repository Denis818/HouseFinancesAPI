using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;

namespace Data.Repository.Categorias
{
    public class GrupoDespesaRepository(IServiceProvider service)
        : RepositoryBase<GrupoDespesa, FinanceDbContext>(service),
            IGrupoDespesaRepository
    {
    }
}
