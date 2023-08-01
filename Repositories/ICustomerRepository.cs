using Customer.WebApi.Models.Customer;
using Dapper;

namespace Customer.WebApi.Services
{
    public interface ICustomerRepository
    {
        Task<List<CustomerDto>> GetCustomersAsync();
        Task InsertCustomersAsync(List<CustomerDto> list);
        Task<CustomerDto> UpdateCustomerAsync(CustomerDto toUpdate);
    }

    public class CustomerRepository: ICustomerRepository
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogRepository _logRepository;

        public CustomerRepository(
            IConnectionProvider connectionProvider, 
            ILogRepository logRepository)
        {
            _connectionProvider = connectionProvider;
            _logRepository = logRepository;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            var getCustomersSqlCommand = "SELECT * FROM dbo.Customer";
            using var sqlConnection = _connectionProvider.GetConnection();
            sqlConnection.Open();
            var customers = await sqlConnection.QueryAsync<CustomerDto>(getCustomersSqlCommand);
            return customers.ToList();
        }

        public async Task InsertCustomersAsync(List<CustomerDto> list)
        {
            using var sqlConnection = _connectionProvider.GetConnection();
            sqlConnection.Open();
            foreach (var item in list)
            {
                await InsertCustomerAsync(item);
                if (item.Id > 0)
                {
                    await _logRepository.InsertLogAsync(item, Models.Action.Create);
                }
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customerToUpdate)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();
            var sql = "UPDATE dbo.Customer set PostCode = @PostCode WHERE Id = @Id";
            int rowsAffected = await connection.ExecuteAsync(sql, customerToUpdate);
            if (rowsAffected > 0)
            {
                await _logRepository.InsertLogAsync(customerToUpdate, Models.Action.Update);
            }

            return customerToUpdate;
        }

        private async Task<CustomerDto> InsertCustomerAsync(CustomerDto customerToInsert)
        {
            using var sqlConnection = _connectionProvider.GetConnection();
            sqlConnection.Open();
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
