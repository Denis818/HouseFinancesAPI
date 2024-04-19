using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Application.Helpers
{
    public class PasswordHasherHelper
    {
        public (string Salt, string Hash) CriarHashSenha(string senha)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: senha,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                )
            );

            return (Convert.ToBase64String(salt), hash);
        }

        public string CompareHash(string senha, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: senha,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                )
            );
        }
    }
}
