using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FruitsStoreBackendASPNET.Data;
using FruitsStoreBackendASPNET.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace FruitsStoreBackendASPNET.Helpers
{
    public class AuthHelper(IConfiguration configuration)
    {
        private readonly DataContextDapper _dapper = new DataContextDapper(configuration);
        private readonly IConfiguration _configuration = configuration;
        private readonly AuthService _authService = new AuthService(configuration);

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

        public string CreateLoginAndSignUpToken(Guid userId)
        {
            return CreateToken(userId, "AppSettings:TokenKey", 2);
        }

        public Guid CreateResetPasswordGUID(Guid userId)
        {
            var Token = CreateToken(userId, "AppSettings:ResetPasswordToken", 1);
            string combined = Token + Guid.NewGuid().ToString();
            using SHA256 sha256Hash = SHA256.Create();
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combined));

            byte[] guidBytes = new byte[16];
            Array.Copy(hash, guidBytes, 16);
            Guid newToken = new Guid(guidBytes);
            _authService.SaveResetGuidInDataBase(userId, newToken);
            return newToken;
        }

        private string CreateToken(
            Guid userId,
            string tokenKeyStringParameter,
            int ExpiryOfTheToken
        )
        {
            Claim[] claims = [new Claim("userId", userId.ToString())];

            string? tokenKeyString = _configuration.GetSection(tokenKeyStringParameter).Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKeyString ?? "")
            );

            SigningCredentials credentials = new SigningCredentials(
                tokenKey,
                SecurityAlgorithms.HmacSha512Signature
            );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddHours(ExpiryOfTheToken),
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
