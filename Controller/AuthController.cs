using System.Data;
using System.Security.Cryptography;
using FruitsStoreBackendASPNET.Data;
using FruitsStoreBackendASPNET.Dtos;
using FruitsStoreBackendASPNET.Helpers;
using FruitsStoreBackendASPNET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FruitsStoreBackendASPNET.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        private readonly DataContextDapper _dapper = new DataContextDapper(configuration);

        private readonly AuthHelper _authHelper = new AuthHelper(configuration);

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
        {
            // Verify whether the passwords match

            if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
            {
                string SQlCheckIfTheUserExists =
                    "Select Email from FruitsStoreBackendSchema.Auth WHERE Email  = '"
                    + userForRegistrationDto.Email
                    + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(
                    SQlCheckIfTheUserExists
                );

                // Verify whether the email address is registered previously

                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(
                        userForRegistrationDto.Password,
                        passwordSalt
                    );

                    string sqlAddAuth =
                        @"INSERT INTO FruitsStoreBackendSchema.AUTH ([Email],
                                        [passwordHash],
                                        [passwordSalt]) VALUES('"
                        + userForRegistrationDto.Email
                        + "', @passwordHash, @passwordSalt)";

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

                    if (_dapper.ExecuteSqlWithListParameters(sqlAddAuth, SqlParameters))
                    {
                        string SQLAddAUser =
                            @"INSERT INTO FruitsStoreBackendSchema.Users(
                                     [FirstName],   
                                     [LastName],   
                                     [Email],   
                                     [Gender],   
                                     [Active],
                                     [CreatedAt]
                                    ) VALUES("
                            + "'"
                            + userForRegistrationDto.FirstName
                            + "',  '"
                            + userForRegistrationDto.LastName
                            + "',  '"
                            + userForRegistrationDto.Email
                            + "',  '"
                            + userForRegistrationDto.Gender
                            + "' , 1"
                            + ",  '"
                            + DateTime.Now
                            + "')";
                        if (_dapper.ExecuteSql(SQLAddAUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to Add User");
                    }
                    throw new Exception("Failed to Create User");
                }
                throw new Exception("There is already a user with this email.");
            }
            throw new Exception(
                "There is a mismatch between the password and the confirm passwords."
            );
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public IActionResult SignUp(UserForSignUpDto userForSignUpDto)
        {
            // Verify whether the email address is registered previously

            string SQlCheckIfTheUserExists =
                "Select Email from FruitsStoreBackendSchema.Auth WHERE Email  = '"
                + userForSignUpDto.Email
                + "'";
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(SQlCheckIfTheUserExists);

            if (existingUsers.Count() == 0)
            {
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = _authHelper.GetPasswordHash(
                    userForSignUpDto.Password,
                    passwordSalt
                );

                string sqlAddAuth =
                    @"INSERT INTO FruitsStoreBackendSchema.AUTH ([Email],
                                        [passwordHash],
                                        [passwordSalt]) VALUES('"
                    + userForSignUpDto.Email
                    + "', @passwordHash, @passwordSalt)";

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

                if (_dapper.ExecuteSqlWithListParameters(sqlAddAuth, SqlParameters))
                {
                    string SQLAddAUser =
                        @"INSERT INTO FruitsStoreBackendSchema.Users(
                                     [Email] 
                                    ) VALUES("
                        + "'"
                        + userForSignUpDto.Email
                        + "')";

                    if (_dapper.ExecuteSql(SQLAddAUser))
                    {
                        string userIdSql =
                            @"SELECT UserId FROM FruitsStoreBackendSchema.Users WHERE Email = '"
                            + userForSignUpDto.Email
                            + "'";

                        Guid userId = _dapper.LoadDataSingle<Guid>(userIdSql);

                        return Ok(
                            new Dictionary<string, string>
                            {
                                { "token", _authHelper.CreateToken(userId) },
                            }
                        );
                    }
                    throw new Exception("Failed to Add User");
                }
                throw new Exception("Failed to Create User");
            }
            throw new Exception("There is already a user with this email.");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            string sqlForHashAndSalt =
                @"SELECT [PasswordHash], [PasswordSalt] 
                FROM FruitsStoreBackendSchema.Auth WHERE Email = '"
                + userForLoginDto.Email
                + "'";

            UserForLoginConfirmationDetailsDto userForLoginConfirmationDetailsDto =
                _dapper.LoadDataSingle<UserForLoginConfirmationDetailsDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(
                userForLoginDto.Password,
                userForLoginConfirmationDetailsDto.PasswordSalt
            );

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForLoginConfirmationDetailsDto.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password");
                }
            }

            string userIdSql =
                @"SELECT UserId FROM FruitsStoreBackendSchema.Users WHERE Email = '"
                + userForLoginDto.Email
                + "'";

            Guid userId = _dapper.LoadDataSingle<Guid>(userIdSql);

            return Ok(
                new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } }
            );
        }
    }
}
