using Domain.Enumeradores;

namespace Domain.Dtos.User.Auth
{
    public record AddUserPermissionDto(int UsuarioId, EnumPermissoes[] Permissoes);
}
