using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using fruits_store_backend_asp_net.Data;
using fruits_store_backend_asp_net.Dtos;
using fruits_store_backend_asp_net.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace fruits_store_backend_asp_net.Helpers
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
            if (_authService.SaveResetGuidInDataBase(userId, newToken))
            {
                return newToken;
            }
            throw new Exception("Failed to Create Reset Password Guid");
        }

        public bool CreateHashPassword(
            UserForSignUpDto userForSignUpDto,
            string reason,
            string? userEmail = null
        )
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForSignUpDto.Password, passwordSalt);
            string sqlAddAuth = "";
            if (reason == "SignUp")
            {
                sqlAddAuth =
                    @"INSERT INTO FruitsStoreBackendSchema.AUTH ([Email],
                                        [passwordHash],
                                        [passwordSalt]) VALUES('"
                    + userForSignUpDto.Email
                    + "', @passwordHash, @passwordSalt)";
            }
            else
            {
                sqlAddAuth =
                    @"Update FruitsStoreBackendSchema.AUTH  SET  passwordHash = @passwordHash
                    ,  passwordSalt =  @passwordSalt WHERE Email = '"
                    + userEmail
                    + "'";
            }

            List<SqlParameter> SqlParameters = new List<SqlParameter>();

            SqlParameter passwordSaltParameter = new SqlParameter(
                "@PasswordSalt",
                SqlDbType.VarBinary
            );
            passwordSaltParameter.Value = passwordSalt;

            SqlParameter passwordHashParameter = new SqlParameter(
                "@PasswordHash",
                SqlDbType.VarBinary
            );
            passwordHashParameter.Value = passwordHash;

            SqlParameters.Add(passwordHashParameter);
            SqlParameters.Add(passwordSaltParameter);

            return _dapper.ExecuteSqlWithListParameters(sqlAddAuth, SqlParameters);
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
