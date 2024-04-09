using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Services.User;
using Application.Services.Base;
using Domain.Dtos.User;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Users;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.User
{
    public class AuthService(IServiceProvider service, IConfiguration _configuration)
        : BaseService<Usuario, IUsuarioRepository>(service),
            IAuthService
    {
        private readonly PasswordHasher PasswordHasher = new();

        public async Task CadastrarUsuario(UserDto userDto)
        {
            if (Validator(userDto))
                return;

            var (Salt, passwordHash) = PasswordHasher.CriarHashSenha(userDto.Password);

            Usuario novoUsuario =
                new()
                {
                    Email = userDto.Email,
                    Password = passwordHash,
                    Salt = Salt
                };

            await _repository.InsertAsync(novoUsuario);
            await _repository.SaveChangesAsync();
        }

        public async Task<UserTokenDto> AutenticarUsuario(UserDto userDto)
        {
            var usuario = await _repository
                .Get()
                .Include(c => c.Permissoes)
                .SingleOrDefaultAsync(u => u.Email == userDto.Email);

            if (usuario == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email não encontrado.");
                return null;
            }

            bool senhaValida = VerificarSenhaHash(userDto.Password, usuario.Password, usuario.Salt);

            if (!senhaValida)
            {
                Notificar(EnumTipoNotificacao.ClientError, "Senha inválida.");
                return null;
            }

            return GerarToken(usuario);
        }

        public bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar)
        {
            var permissoes = _httpContext?.User?.Claims?.Select(claim => claim.Value.ToString());

            var possuiPermissao = permissoesParaValidar
                .Select(permissao => permissao.ToString())
                .All(permissao => permissoes.Any(x => x == permissao));

            return possuiPermissao;
        }

        public async Task AddPermissaoAsync(AddUserPermissionDto userPermissao) =>
            await _repository.AddPermissaoAsync(userPermissao);

        #region Supports Methods
        private bool VerificarSenhaHash(string senha, string senhaHash, string salt) =>
            PasswordHasher.CompareHash(senha, salt) == senhaHash;

        private UserTokenDto GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenExpirationTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["TokenConfiguration:ExpireDays"])
            );

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (usuario.Permissoes.Count > 0)
            {
                foreach (var permissao in usuario.Permissoes)
                {
                    claims.Add(new Claim("Permission", permissao.Descricao));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpirationTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Audience = _configuration["TokenConfiguration:Audience"],
                Issuer = _configuration["TokenConfiguration:Issuer"]
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new UserTokenDto()
            {
                Authenticated = true,
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenExpirationTime,
            };
        }
        #endregion
    }
}
