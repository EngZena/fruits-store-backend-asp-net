using System.Text;
using FruitsStoreBackendASPNET.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace FruitsStoreBackendASPNET.Helpers
{
    public class AuthHelper
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _configuration;
        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _dapper = new DataContextDapper(configuration);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value
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