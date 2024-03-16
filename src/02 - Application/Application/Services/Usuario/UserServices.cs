using Application.Interfaces.Services;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Usuario
{
    public class UserServices(IHttpContextAccessor acessor) : IUserServices
    {
        private readonly IHttpContextAccessor _acessor = acessor;
        private readonly IEnumerable<string> _permissoes = acessor.HttpContext?.User?.Claims?.Select(claim => claim.Value.ToString());

        public string Name => _acessor.HttpContext.User.Identity.Name;

        public bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar)
        {
            var possuiPermissao = permissoesParaValidar
                .Select(permissao => permissao.ToString())
                .All(permissao => _permissoes.Any(x => x == permissao));

            return possuiPermissao;
        }
    }
}
