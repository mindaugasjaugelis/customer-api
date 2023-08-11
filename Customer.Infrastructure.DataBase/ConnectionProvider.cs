using System.Threading.Tasks;
using Customer.Infrastructure.DataBase.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Customer.Infrastructure.DataBase
{
    public class SqlConnectionProvider : ISqlConnectionProvider
    {
        private string _connectionString;

        public SqlConnectionProvider(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions)
        {
            _connectionString = connectionStringOptions.CurrentValue.ConnectionString;
        }

        public async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
