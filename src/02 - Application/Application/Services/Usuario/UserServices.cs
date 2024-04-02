using Application.Interfaces.Services.User;
using Domain.Dtos.User;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Usuario
{
    public class UserServices(IHttpContextAccessor Acessor, IConfiguration Configuration) : IUserServices
    {
        private readonly int TokenExpire = int.Parse(Configuration["TokenConfiguration:ExpireHours"]);
        public string Name => Acessor.HttpContext.User.Identity.Name;

        public bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar)
        {
           var permissoes = Acessor.HttpContext?.User?
                                   .Claims?.Select(claim => claim.Value.ToString());

            var possuiPermissao = permissoesParaValidar
                .Select(permissao => permissao.ToString())
                .All(permissao => permissoes.Any(x => x == permissao));

            return possuiPermissao;
        }

        public UserTokenDto GerarToken(UserDto userDto, Claim[] permissoes)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, userDto.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(permissoes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:key"]));

            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationFormat = DateTime.UtcNow.AddDays(TokenExpire);

            JwtSecurityToken token = new(
              issuer: Configuration["TokenConfiguration:Issuer"],
              audience: Configuration["TokenConfiguration:Audience"],
              claims: claims,
              expires: expirationFormat,
              signingCredentials: credenciais);

            return new UserTokenDto()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expirationFormat,
            };
        }

    }
}
