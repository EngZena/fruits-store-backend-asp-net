using System.Data;
using System.Security.Cryptography;
using FruitsStoreBackendASPNET.Data;
using FruitsStoreBackendASPNET.Dtos;
using FruitsStoreBackendASPNET.Helpers;
using FruitsStoreBackendASPNET.Models;
using FruitsStoreBackendASPNET.Services;
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
        private readonly AuthService _authService = new AuthService(configuration);

        /// <summary>
        /// Creates a new user account using some details
        /// </summary>
        /// <remarks>
        /// This API registers a new user by storing their credentials and associated details
        /// </remarks>
        /// <param name="userForRegistrationDto">User registration details</param>
        /// <returns>Returns a success message if the account is created, or an error if the email is already in use</returns>
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

        /// <summary>
        /// Creates a new user account using an email and password
        /// </summary>
        /// <remarks>
        /// This API registers a new user
        /// </remarks>
        /// <param name="userForSignUpDto">User signup details</param>
        /// <returns>Returns a success message if the account is created, or an error if the email is already in use</returns>
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
                if (_authHelper.CreateHashPassword(userForSignUpDto, "SignUp"))
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
                        Guid userId = _authService.GetUserGuidByEmail(userForSignUpDto.Email);

                        return Ok(
                            new Dictionary<string, string>
                            {
                                { "token", _authHelper.CreateLoginAndSignUpToken(userId) },
                            }
                        );
                    }
                    throw new Exception("Failed to Add User");
                }
                throw new Exception("Failed to Create User");
            }
            throw new Exception("There is already a user with this email.");
        }

        /// <summary>
        /// Authenticates a user and generates an access token
        /// </summary>
        /// <remarks>
        /// This API verifies the user's credentials (email and password)
        /// If authentication is successful, a JWT token is returned for future requests
        /// </remarks>
        /// <param name="userForLoginDto">User login credentials</param>
        /// <returns>Returns a JWT token on successful authentication, or an error for invalid credentials</returns>

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
            if (userForLoginConfirmationDetailsDto == null)
            {
                return StatusCode(400, "Incorrect Email");
            }
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

            Guid userId = _authService.GetUserGuidByEmail(userForLoginDto.Email);

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", _authHelper.CreateLoginAndSignUpToken(userId) },
                }
            );
        }

        /// <summary>
        /// Returns a GUID to submit new password
        /// </summary>
        /// <remarks>
        /// This API returns a GUID to submit new password using user Email
        /// If email is correct, a GUID is returned
        /// </remarks>
        /// <param name="userEmail">email@gmail.com</param>
        /// <returns>Returns a GUID token for correct email, or an error for incorrect email</returns>

        [HttpPost("RequestResetPassword")]
        public IActionResult RequestResetPassword(string userEmail)
        {
            Guid userId = _authService.GetUserGuidByEmail(userEmail);
            if (userId == Guid.Empty)
            {
                return StatusCode(404, "The Provided Email is Invalid");
            }
            else
            {
                if (!_authService.IsNumberOfAttemptsWithinLimitAndTheGuidIsValid(userId))
                {
                    return StatusCode(404, "Please try after one hour");
                }
                return Ok(_authHelper.CreateResetPasswordGUID(userId));
            }
        }

        /// <summary>
        /// Submit New Password using secret GUID
        /// </summary>
        /// <remarks>
        /// This API Submit New Password using secret GUID
        /// If user GUID and new password matching the confirm password then the user password will be changed.
        /// </remarks>
        /// <param name="userGuid">00000000-0000-0000-0000-000000000000</param>
        /// <param name="userEmail">email@gmail.com</param>
        /// <param name="password">New password</param>
        /// <param name="conformationPassword">Conformation Password</param>
        /// <returns>Change user Password if the provided data correct, or an error for incorrect data.</returns>

        [HttpPost("SubmitNewPassword/{userGuid}/{userEmail}")]
        public IActionResult SubmitNewPassword(
            Guid userGuid,
            string userEmail,
            string password,
            string conformationPassword
        )
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId != null && Guid.TryParse(userId, out Guid userIdFromGuid))
            {
                var ResetPasswordUserGuid = _authService.GetResetPasswordUserGuidByUserId(
                    userIdFromGuid
                );
                if (ResetPasswordUserGuid != userIdFromGuid)
                {
                    return StatusCode(404, "The Provided GUID is Invalid");
                }
                if (password != conformationPassword)
                {
                    return StatusCode(
                        404,
                        "There is a mismatch between the password and the confirm passwords."
                    );
                }
                if (_authService.IsNumberOfAttemptsWithinLimitAndTheGuidIsValid(userGuid))
                {
                    if (_authService.UpdateResetPasswordValidityByUserId(userIdFromGuid))
                    {
                        var userEmail2 = _authService.GetUserEmailByUserId(userGuid);
                        var userForResetPassword = new UserForSignUpDto(userEmail, password);
                        if (
                            !_authHelper.CreateHashPassword(
                                userForResetPassword,
                                "SubmitNewPassword",
                                userEmail
                            )
                        )
                        {
                            throw new Exception("Failed to Update Password");
                        }
                    }
                }
                else
                {
                    return StatusCode(404, "Please try after one hour");
                }
                return Ok("Password updated Successfully");
            }
            return StatusCode(401, "Something went wrong");
        }
    }
}
