using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces;
using Domain.Models;

namespace Data.Repository.Finance
{
    public class DespesaRepository(IServiceProvider service) : 
        RepositoryBase<Despesa, FinanceDbContext>(service), IDespesaRepository
    {
    }
}
