using Domain.Dtos.User;
using Domain.Enumeradores;
using System.Security.Claims;

namespace Application.Interfaces.Services
{
    public interface IUserServices
    {
        public string Name { get; }
        bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar);
        UserTokenDto GerarToken(UserDto userDto, Claim[] permissoes);
    }
}
