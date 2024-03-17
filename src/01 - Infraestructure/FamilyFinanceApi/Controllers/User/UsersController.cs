using Application.Interfaces.Services;
using Domain.Dtos.User;
using Domain.Enumeradores;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        IServiceProvider service,
        IUserServices userService) : BaseApiController(service)
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserServices _userService = userService;

        [HttpPost("login")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserTokenExample))]
        public async Task<UserTokenDto> Login(UserDto userDto)
        {
            var userLogin = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password,
                                                                     isPersistent: false, lockoutOnFailure: false);

            if (!userLogin.Succeeded)
            {
                Notificar("Email ou Senha incorretos.", EnumTipoNotificacao.ClientError);
                return null;
            }

            var user = await _userManager.FindByEmailAsync(userDto.Email);
            var claims = await _userManager.GetClaimsAsync(user);

            return GerarToken(userDto, claims.ToArray());
        }

        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo() => new(_userService.Name, _userService.PossuiPermissao(EnumPermissoes.USU_000001));

        [HttpGet("logout")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Logout() => await _signInManager.SignOutAsync();

        private UserTokenDto GerarToken(UserDto userDto, Claim[] permissoes)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, userDto.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(permissoes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationFormat = DateTime.UtcNow.AddHours(int.Parse(_configuration["TokenConfiguration:ExpireHours"]));

            JwtSecurityToken token = new(
              issuer: _configuration["TokenConfiguration:Issuer"],
              audience: _configuration["TokenConfiguration:Audience"],
              claims: claims,
              expires: expirationFormat,
              signingCredentials: credenciais);

            Notificar("Data de expiração no formato UTC.", EnumTipoNotificacao.Informacao);

            return new UserTokenDto()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expirationFormat,
            };
        }
    }
}
