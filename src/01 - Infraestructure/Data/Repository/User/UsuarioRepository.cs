using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Models.Users;

namespace Data.Repository.User
{
    public class UsuarioRepository(IServiceProvider service) :
        RepositoryBase<Usuario, FinanceDbContext>(service), IUsuarioRepository
    {

    }
}
