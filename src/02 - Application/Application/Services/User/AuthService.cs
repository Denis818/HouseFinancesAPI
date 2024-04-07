using Application.Interfaces.Services.User;
using Application.Services.Base;
using Domain.Dtos.User;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Users;
using Domain.Utilities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.User
{
    public class AuthService(
        IHttpContextAccessor Acessor,
        IConfiguration _configuration,
        IServiceProvider Service, PasswordHasher hashService)
        : BaseService<Usuario, UserDto, IUsuarioRepository>(Service), IAuthService
    {
        public string Name => Acessor.HttpContext.User.Identity.Name;


        public async Task CadastrarUsuario(UserDto userDto)
        {
            if (Validator(userDto)) return;

            var (Salt, passwordHash) = hashService.CriarHashSenha(userDto.Password);

            Usuario novoUsuario = new()
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
            var usuario = await _repository.Get().Include(c => c.Permissoes)
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
            var permissoes = Acessor.HttpContext?.User?
                                    .Claims?.Select(claim => claim.Value.ToString());

            var possuiPermissao = permissoesParaValidar
                .Select(permissao => permissao.ToString())
                .All(permissao => permissoes.Any(x => x == permissao));

            return possuiPermissao;
        }

        public async Task AddPermissaoAsync(int usuarioId, params EnumPermissoes[] permissoes)
        {
            var usuario = await _repository.Get(user => user.Id == usuarioId)
                                 .Include(p => p.Permissoes)
                                 .FirstOrDefaultAsync();

            foreach (var permissao in permissoes)
            {
                var possuiPermissao = usuario.Permissoes
                                             .Where(p => p.Nome == permissao.ToString())
                                             .FirstOrDefault();

                if (possuiPermissao is null)
                {
                    usuario.Permissoes.Add(new Permissao { Nome = permissao.ToString() });
                }
            }

            _repository.Update(usuario);
            await _repository.SaveChangesAsync();
        }

        #region Supports Methods
        private bool VerificarSenhaHash(string senha, string senhaHash, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: senha,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == senhaHash;
        }

        private UserTokenDto GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var expirationFormat = DateTime.UtcNow.AddHours(double.Parse(_configuration["TokenConfiguration:ExpireHours"]));
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);

            var claims = new List<Claim>
            {
                 new(ClaimTypes.Name, usuario.Email),
                 new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if(usuario.Permissoes.Count > 0)
            {
                foreach (var permissao in usuario.Permissoes)
                {
                    claims.Add(new Claim("Permission", permissao.Nome));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationFormat,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["TokenConfiguration:Audience"],
                Issuer = _configuration["TokenConfiguration:Issuer"]
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new UserTokenDto()
            {
                Authenticated = true,
                Token = tokenHandler.WriteToken(token),
                Expiration = expirationFormat,
            };
        }
        #endregion
    }
}
