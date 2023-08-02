using Customer.Domain.Models;
using Customer.Domain.Repositories;
using Customer.Infrastructure.DataBase.Abstractions;
using Dapper;
using Microsoft.Data.SqlClient;
using Action = Customer.Domain.Repositories.Models.Action;

namespace Customer.Infrastructure.DataBase.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private readonly ISqlConnectionProvider _connectionProvider;
        private readonly ILogRepository _logRepository;

        public CustomerRepository(
            ISqlConnectionProvider connectionProvider,
            ILogRepository logRepository)
        {
            _connectionProvider = connectionProvider;
            _logRepository = logRepository;
        }

        public async Task<List<CustomerEntity>> GetCustomersAsync()
        {
            var getCustomersSqlCommand = "SELECT * FROM dbo.Customer";
            await using SqlConnection sqlConnection = await _connectionProvider.OpenConnectionAsync();
            IEnumerable<CustomerEntity> customers = await sqlConnection.QueryAsync<CustomerEntity>(getCustomersSqlCommand);
            return customers.ToList();
        }

        public async Task InsertCustomersAsync(List<CustomerEntity> list)
        {
            foreach (var item in list)
            {
                await InsertCustomerAsync(item);
                if (item.Id > 0)
                {
                    await _logRepository.InsertLogAsync(item, Action.Create);
                }
            }
        }

        public async Task<CustomerEntity> UpdateCustomerAsync(CustomerEntity customerToUpdate)
        {
            await using SqlConnection connection = await _connectionProvider.OpenConnectionAsync();
            var sql = "UPDATE dbo.Customer set PostCode = @PostCode WHERE Id = @Id";
            int rowsAffected = await connection.ExecuteAsync(sql, customerToUpdate);
            if (rowsAffected > 0)
            {
                await _logRepository.InsertLogAsync(customerToUpdate, Action.Update);
            }

            return customerToUpdate;
        }

        private async Task<CustomerEntity> InsertCustomerAsync(CustomerEntity customerToInsert)
        {
            await using SqlConnection sqlConnection = await _connectionProvider.OpenConnectionAsync();
            var insertCustomerSqlQuery = "INSERT INTO dbo.Customer (Name, Address, PostCode) OUTPUT INSERTED.Id VALUES (@Name, @Address, @PostCode)";
            var parameters = new
            {
                Name = string.IsNullOrEmpty(customerToInsert.Name) ? (object)DBNull.Value : customerToInsert.Name,
                Address = string.IsNullOrEmpty(customerToInsert.Address) ? (object)DBNull.Value : customerToInsert.Address,
                PostCode = string.IsNullOrEmpty(customerToInsert.PostCode) ? (object)DBNull.Value : customerToInsert.PostCode
            };

            customerToInsert.Id = await sqlConnection.ExecuteScalarAsync<int>(insertCustomerSqlQuery, parameters);
            return customerToInsert;
        }
    }
}
