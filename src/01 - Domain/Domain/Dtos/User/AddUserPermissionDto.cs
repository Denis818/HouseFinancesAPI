using Domain.Enumeradores;

namespace Domain.Dtos.User
{
    public record AddUserPermissionDto(int UsuarioId, EnumPermissoes[] Permissoes);
}
