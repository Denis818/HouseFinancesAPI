using Domain.Dtos.User;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Users;

namespace Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        Task AddPermissaoAsync(AddUserPermissionDto userPermissao);
    }
}
