using Domain.Dtos.User;
using Domain.Enumeradores;

namespace Application.Interfaces.Services.User
{
    public interface IAuthService
    {
        Task<UserTokenDto> AutenticarUsuario(UserDto userDto);
        Task CadastrarUsuario(UserDto userDto);
        bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar);
        Task AddPermissaoAsync(AddUserPermissionDto userPermissao);
    }
}