using FruitsStoreBackendASPNET.Data;
using FruitsStoreBackendASPNET.Models;

namespace FruitsStoreBackendASPNET.Services
{
    public class AuthService(IConfiguration configuration)
    {
        private readonly DataContextDapper _dapper = new DataContextDapper(configuration);

        public Guid GetUserGuidByEmail(string userEmail)
        {
            string userIdSql =
                @"SELECT UserId FROM FruitsStoreBackendSchema.Users WHERE Email = '"
                + userEmail
                + "'";

            return _dapper.LoadDataSingle<Guid>(userIdSql);
        }

        public bool SaveResetGuidInDataBase(Guid userId, Guid resetGuid)
        {
            var SQL_Number_Of_Attempts =
                @"SELECT Number_Of_Attempts FROM FruitsStoreBackendSchema.RequestResetPassword WHERE UserId = '"
                + userId
                + "'";
            int Number_Of_Attempts = _dapper.LoadDataSingle<int>(SQL_Number_Of_Attempts);

            if (Number_Of_Attempts == 0)
            {
                Number_Of_Attempts = 1;
                var createFirstUserLog =
                    @"INSERT INTO FruitsStoreBackendSchema.RequestResetPassword (
                                        [UserId],
                                        [Reset_GUID],
                                        [CreatedAt],
                                        [Number_Of_Attempts],
                                        [IS_VALID]
                                        ) VALUES ('"
                    + userId
                    + "',  '"
                    + resetGuid
                    + "',  '"
                    + DateTime.Now
                    + "',  '"
                    + Number_Of_Attempts
                    + "',  '"
                    + true
                    + "')";
                return _dapper.ExecuteSql(createFirstUserLog);
            }
            else
            {
                Number_Of_Attempts++;
                var updateSql =
                    @"Update FruitsStoreBackendSchema.RequestResetPassword  SET
                    Reset_GUID = '"
                    + resetGuid
                    + "',  CreatedAt = '"
                    + DateTime.Now
                    + "',  Number_Of_Attempts = "
                    + Number_Of_Attempts
                    + ", IS_VALID = 'true'"
                    + " where UserId = '"
                    + userId
                    + "'";
                return _dapper.ExecuteSql(updateSql);
            }
        }

        public bool IsNumberOfAttemptsWithinLimit(Guid userId)
        {
            var SQL_Number_Of_Attempts_And_CreatedAt =
                @"SELECT Number_Of_Attempts, CreatedAt  FROM FruitsStoreBackendSchema.RequestResetPassword WHERE UserId = '"
                + userId
                + "'";

            var Number_Of_Attempts_And_CreatedAt = _dapper.LoadDataSingle<RequestResetPassword>(
                SQL_Number_Of_Attempts_And_CreatedAt
            );

            if (Number_Of_Attempts_And_CreatedAt == null)
            {
                return true;
            }
            else
            {
                return Number_Of_Attempts_And_CreatedAt.Number_Of_Attempts < 5
                    && Number_Of_Attempts_And_CreatedAt.CreatedAt > DateTime.UtcNow.AddHours(-1);
                ;
            }
        }
    }
}
