using Domain.Dtos.User;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Users;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        Task AddPermissaoAsync(AddUserPermissionDto userPermissao);
    }
}
