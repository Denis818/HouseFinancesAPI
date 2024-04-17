using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Finance;

namespace Data.Repository.Finance
{
    public class DespesaRepository(IServiceProvider service)
        : RepositoryBase<Despesa, FinanceDbContext>(service), IDespesaRepository
    { }
}
