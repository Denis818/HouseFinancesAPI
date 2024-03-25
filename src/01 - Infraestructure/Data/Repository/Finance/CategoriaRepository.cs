using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces;
using Domain.Models;

namespace Data.Repository.Finance
{
    public class CategoriaRepository(IServiceProvider service) : 
        RepositoryBase<Categoria, FinanceDbContext>(service), ICategoriaRepository
    {
    }
}
