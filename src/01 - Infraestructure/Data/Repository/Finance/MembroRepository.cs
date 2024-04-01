using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces;
using Domain.Models;

namespace Data.Repository.Finance
{
    public class MembroRepository(IServiceProvider service) : 
        RepositoryBase<Membro, FinanceDbContext>(service), IMembroRepository
    {
    }
}
