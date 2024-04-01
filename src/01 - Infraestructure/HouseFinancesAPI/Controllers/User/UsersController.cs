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
        UserManager<IdentityUser> UserManager,
        SignInManager<IdentityUser> SignInManager,
        IUserServices UserService, 
        IServiceProvider service) : BaseApiController(service)
    {
        [HttpPost("login")]
        public async Task<UserTokenDto> Login(UserDto userDto)
        {
            if(userDto.Email.IsNullOrEmpty() || userDto.Password.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            if (userDto.Email.Replace(" ", "").ToLower() == "master") 
                userDto.Email = _configuration["UserMaster:Email"];

            var userLogin = await SignInManager.PasswordSignInAsync(userDto.Email, userDto.Password,
                                                                     isPersistent: false, lockoutOnFailure: false);

            if (!userLogin.Succeeded)
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            var user = await UserManager.FindByEmailAsync(userDto.Email);
            var claims = await UserManager.GetClaimsAsync(user);

            return UserService.GerarToken(userDto, claims.ToArray());
        }

        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo() => new(UserService.Name, UserService.PossuiPermissao(EnumPermissoes.USU_000001));

        [HttpGet("logout")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Logout() => await SignInManager.SignOutAsync();

    }
}
