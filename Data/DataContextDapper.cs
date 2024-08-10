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
    }
}