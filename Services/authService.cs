using fruits_store_backend_asp_net.Data;
using fruits_store_backend_asp_net.Models;

namespace fruits_store_backend_asp_net.Services
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
            var userIdFromDataBase = _dapper.LoadDataSingle<Guid>(userIdSql);
            if (userIdFromDataBase != Guid.Empty)
            {
                return userIdFromDataBase;
            }
            return Guid.Empty;
        }

        public bool SaveResetGuidInDataBase(Guid userId, Guid resetGuid)
        {
            var SQL_Number_Of_Attempts_And_UserId =
                @"SELECT Number_Of_Attempts FROM FruitsStoreBackendSchema.RequestResetPassword WHERE UserId = '"
                + userId
                + "'";
            int Number_Of_Attempts = _dapper.LoadDataSingle<int>(SQL_Number_Of_Attempts_And_UserId);
            if (Number_Of_Attempts.GetType() != typeof(int))
            {
                return false;
            }
            var sql_statement =
                @"IF EXISTS (SELECT Number_Of_Attempts FROM FruitsStoreBackendSchema.RequestResetPassword WHERE UserId = '"
                + userId
                + "'"
                + ")"
                + @"
                BEGIN 
                Update FruitsStoreBackendSchema.RequestResetPassword  SET
                    Reset_GUID = '"
                + resetGuid
                + "',  CreatedAt = '"
                + DateTime.Now
                + "',  Number_Of_Attempts = "
                + (Number_Of_Attempts + 1)
                + ", IS_VALID = 'true'"
                + " where UserId = '"
                + userId
                + @"' 
                END
                ELSE
                BEGIN
                INSERT INTO FruitsStoreBackendSchema.RequestResetPassword (
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
                + @"')
                 END";
            return _dapper.ExecuteSql(sql_statement);
        }

        public bool IsNumberOfAttemptsWithinLimitAndTheGuidIsValid(Guid userId)
        {
            var SQL_Number_Of_Attempts_And_CreatedAt =
                @"SELECT Number_Of_Attempts, CreatedAt FROM FruitsStoreBackendSchema.RequestResetPassword WHERE UserId = '"
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
                if (Number_Of_Attempts_And_CreatedAt.CreatedAt < DateTime.UtcNow.AddHours(-2))
                {
                    var updateSql =
                        @"Update FruitsStoreBackendSchema.RequestResetPassword  SET  Number_Of_Attempts = 0 WHERE UserId = '"
                        + userId
                        + "'";
                    _dapper.ExecuteSql(updateSql);
                    Number_Of_Attempts_And_CreatedAt = _dapper.LoadDataSingle<RequestResetPassword>(
                        SQL_Number_Of_Attempts_And_CreatedAt
                    );
                    if (Number_Of_Attempts_And_CreatedAt.GetType() == null)
                    {
                        return false;
                    }
                }
                return Number_Of_Attempts_And_CreatedAt.Number_Of_Attempts < 5;
            }
        }

        public Guid GetResetPasswordUserGuidByUserId(Guid userId)
        {
            string userIdSql =
                @"SELECT UserId FROM FruitsStoreBackendSchema.RequestResetPassword WHERE UserId = '"
                + userId
                + "'";
            var userIdFromDataBase = _dapper.LoadDataSingle<Guid>(userIdSql);
            if (userIdFromDataBase == Guid.Empty)
            {
                return Guid.Empty;
            }
            if (userIdSql != null)
            {
                return userIdFromDataBase;
            }
            return Guid.Empty;
        }

        public bool UpdateResetPasswordValidityByUserId(Guid userId)
        {
            var updateSql =
                @"Update FruitsStoreBackendSchema.RequestResetPassword  SET  IS_VALID = 'false' WHERE UserId = '"
                + userId
                + "'";
            return _dapper.ExecuteSql(updateSql);
        }

        public string GetUserEmailByUserId(Guid userId)
        {
            string userIdSql =
                @"SELECT Email FROM FruitsStoreBackendSchema.Users WHERE UserId = '" + userId + "'";
            var userEmail = _dapper.LoadDataSingle<string>(userIdSql);
            if (userEmail == "" || userEmail == null)
            {
                return "user not found";
            }
            return userEmail;
        }

        public Guid GetUserIdByUserGuidId(Guid userGuidId)
        {
            string userIdSql =
                @"SELECT userId FROM FruitsStoreBackendSchema.RequestResetPassword WHERE Reset_GUID = '"
                + userGuidId
                + "'";
            var userId = _dapper.LoadDataSingle<Guid>(userIdSql);
            if (userId != Guid.Empty)
            {
                return userId;
            }
            return Guid.Empty;
        }
    }
}
