using Domain.Dtos.User;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;

namespace Application.Interfaces.Services.User
{
    public interface IAuthAppService
    {
        Task<UserTokenDto> AutenticarUsuario(UserDto userDto);
        bool VerificarPermissao(params EnumPermissoes[] permissoesParaValidar);
        Task AddPermissaoAsync(AddUserPermissionDto userPermissao);
    }
}
