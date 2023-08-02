using Microsoft.Data.SqlClient;

namespace Customer.Infrastructure.DataBase.Abstractions
{
    public interface ISqlConnectionProvider
    {
        Task<SqlConnection> OpenConnectionAsync();
    }
}
