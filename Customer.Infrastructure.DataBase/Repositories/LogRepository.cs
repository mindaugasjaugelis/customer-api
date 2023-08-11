using System.Threading.Tasks;
using System.Text.Json;
using Customer.Domain.Models;
using Customer.Domain.Repositories;
using Customer.Infrastructure.DataBase.Abstractions;
using Action = Customer.Domain.Repositories.Models.Action;
using Dapper;
using Customer.Domain.Repositories.Models;
using Microsoft.Data.SqlClient;

namespace Customer.Infrastructure.DataBase.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ISqlConnectionProvider _connectionProvider;

        public LogRepository(ISqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task InsertLogAsync(CustomerEntity item, Action action)
        {
            await using SqlConnection sqlConnection = await _connectionProvider.OpenConnectionAsync();
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
