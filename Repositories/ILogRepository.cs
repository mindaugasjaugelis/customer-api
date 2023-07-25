using System.Text.Json;
using Customer.WebApi.Models;
using Customer.WebApi.Models.Customer;
using Dapper;

namespace Customer.WebApi.Services
{
    public interface ILogRepository
    {
        void Insert(CustomerDto item, Models.Action action);
    }

    public class LogRepository : ILogRepository
    {
        private readonly IConnectionProvider _connectionProvider;

        public LogRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void Insert(CustomerDto item, Models.Action action)
        {
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();

            string sql = @"INSERT INTO dbo.Log (EntityTypeId, EntityId, ActionId, LogCreatedAt, EntityJson)
                VALUES (@EntityTypeId, @EntityId, @ActionId, GETDATE(), @EntityJson)";
            var parameters = new
            {
                EntityTypeId = (int)EntityType.Customer,
                EntityId = item.Id,
                ActionId = (int)action,
                EntityJson = JsonSerializer.Serialize(item)
            };

            connection.Execute(sql, parameters);
        }
    }
}
