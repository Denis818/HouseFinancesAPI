using Domain.Dtos.User;
using Domain.Enumeradores;

namespace Application.Interfaces.Services.User
{
    public interface IAuthService
    {
        string Name { get; }
        Task<UserTokenDto> AutenticarUsuario(UserDto userDto);
        Task CadastrarUsuario(UserDto userDto);
        bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar);
    }
}