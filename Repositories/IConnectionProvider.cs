using System.Data.SqlClient;

namespace Customer.WebApi.Services
{
    public interface IConnectionProvider
    {
        public SqlConnection GetSqlConnection();
    }

    public class ConnectionProvider : IConnectionProvider
    {
        private readonly string ConnectionStringName = "CustomConnectionString";

        private readonly IConfiguration _configuration;

        public ConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetValue<string>(ConnectionStringName));
        }
    }
}
