using System.Text.Json;
using Customer.WebApi.Models;
using Customer.WebApi.Models.Customer;
using Dapper;

namespace Customer.WebApi.Services
{
    public interface ILogRepository
    {
        Task InsertLogAsync(CustomerDto item, Models.Action action);
    }

    public class LogRepository : ILogRepository
    {
        private readonly IConnectionProvider _connectionProvider;

        public LogRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task InsertLogAsync(CustomerDto item, Models.Action action)
        {
            using var sqlConnection = _connectionProvider.GetConnection();
            sqlConnection.Open();
            string insertLogSqlQuery = @"INSERT INTO dbo.Log (EntityTypeId, EntityId, ActionId, LogCreatedAt, EntityJson)
                VALUES (@EntityTypeId, @EntityId, @ActionId, GETDATE(), @EntityJson)";
            var parameters = new
            {
                EntityTypeId = (int)EntityType.Customer,
                EntityId = item.Id,
                ActionId = (int)action,
                EntityJson = JsonSerializer.Serialize(item)
            };

            await sqlConnection.ExecuteAsync(insertLogSqlQuery, parameters);
        }
    }
}
