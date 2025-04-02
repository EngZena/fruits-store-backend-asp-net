using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace FruitsStoreBackendASPNET.Data
{
    public class DataContextDapper(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );
            return dbConnection.Query<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );
            return dbConnection.Execute(sql) > 0;
        }

        public bool ExecuteSqlWithListParameters(string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParam = new SqlCommand(sql);
            foreach (SqlParameter parameter in parameters)
            {
                commandWithParam.Parameters.Add(parameter);
            }

            SqlConnection dbConnection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );

            dbConnection.Open();
            commandWithParam.Connection = dbConnection;

            int rowsAffected = commandWithParam.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }

#nullable disable
        public T LoadDataSingle<T>(string sql)
        {
            string defaultConnection =
                _configuration.GetConnectionString("DefaultConnection") ?? "";
            IDbConnection dbConnection = new SqlConnection(defaultConnection);
            return dbConnection.QueryFirstOrDefault<T>(sql);
        }
#nullable restore
    }
}
