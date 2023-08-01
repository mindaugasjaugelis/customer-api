using System.Data.SqlClient;

namespace Customer.WebApi.Services
{
    public interface IConnectionProvider
    {
        public SqlConnection GetConnection();
    }

    public class SqlConnectionProvider : IConnectionProvider
    {
        private static readonly string ConnectionStringName = "CustomConnectionString";

        private readonly IConfiguration _configuration;

        public SqlConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetValue<string>(ConnectionStringName));
        }
    }
}
