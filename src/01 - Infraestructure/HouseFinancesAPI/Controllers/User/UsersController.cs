using Domain.Dtos.User;
using Domain.Enumeradores;
using HouseFinancesAPI.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using HouseFinancesAPI.Controllers.Base;
using Application.Interfaces.Services.User;

namespace Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        IAuthService _authService,
        IServiceProvider service) : BaseApiController(service)
    {
        [HttpPost("login")]
        public async Task<UserTokenDto> Login(UserDto userDto)
        {
            if (userDto.Email.IsNullOrEmpty() || userDto.Password.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            if (userDto.Email.Replace(" ", "").ToLower() == "master")
                userDto.Email = _configuration["UserMaster:Email"];

            return await _authService.AutenticarUsuario(userDto);
        }

        [HttpGet("add-permission")]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task AddPermissaoAsync(AddUserPermissionDto userPermissao)
            => await _authService.AddPermissaoAsync(userPermissao);

        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo()
            => new(HttpContext.User.Identity.Name, _authService.PossuiPermissao(EnumPermissoes.USU_000001));
    }
}
