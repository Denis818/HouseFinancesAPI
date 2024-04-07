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
        IAuthService UserService,
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

            var token = await UserService.AutenticarUsuario(userDto);

            if (token == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            return token;
        }

        [HttpGet("add-permission")]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task AddPermissaoAsync(int idUsuario, params EnumPermissoes[] permissoes) 
            => await UserService.AddPermissaoAsync(idUsuario, permissoes);
        
        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo() 
            => new(UserService.Name, UserService.PossuiPermissao(EnumPermissoes.USU_000001));
    }
}
