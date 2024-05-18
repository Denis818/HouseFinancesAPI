using Data.DataContext;
using Domain.Interfaces.Repositories;
using Domain.Models.Users;
using Infraestructure.Data.Repository.Base;

namespace Data.Repository.User
{
    public class UsuarioRepository(IServiceProvider service)
        : RepositoryBase<Usuario, FinanceDbContext>(service),
            IUsuarioRepository { }
}
