using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces.Repository.Finance;
using Domain.Models.Finance;

namespace Data.Repository.Finance
{
    public class FinanceRepository(IServiceProvider service) : 
        RepositoryBase<Despesa, FinanceDbContext>(service), IFinanceRepository
    {
    }
}
