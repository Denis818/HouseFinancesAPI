using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Interfaces;
using Domain.Models;

namespace Data.Repository.Finance
{
    public class MemberRepository(IServiceProvider service) : 
        RepositoryBase<Member, FinanceDbContext>(service), IMemberRepository
    {
    }
}
