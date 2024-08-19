using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FruitsStoreBackendASPNET.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace FruitsStoreBackendASPNET.Helpers
{
    public class AuthHelper(IConfiguration configuration)
    {
        private readonly DataContextDapper _dapper = new DataContextDapper(configuration);
        private readonly IConfiguration _configuration = configuration;

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString =
                _configuration.GetSection("AppSettings:PasswordKey").Value
                + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }
    }
}
