using Customer.WebApi.Models.Customer;
using Dapper;

namespace Customer.WebApi.Services
{
    public interface ICustomerRepository
    {
        List<CustomerDto> GetList();
        void Insert(List<CustomerDto> list);
        void Update(CustomerDto toUpdate);
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

        public List<CustomerDto> GetList()
        {
            string query = "SELECT * FROM dbo.Customer";
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();
            return connection.Query<CustomerDto>(query).ToList();
        }

        public void Insert(List<CustomerDto> list)
        {
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();
            foreach (var item in list)
            {
                Insert(item);
                if (item.Id > 0)
                {
                    _logRepository.Insert(item, Models.Action.Create);
                }
            }
        }

        public void Update(CustomerDto item)
        {
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();

            string sql = "UPDATE dbo.Customer set PostCode = @PostCode WHERE Id = @Id";
            if(connection.Execute(sql, item) > 0)
            {
                _logRepository.Insert(item, Models.Action.Update);
            }
        }

        private void Insert(CustomerDto item)
        {
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();

            string sql = "INSERT INTO dbo.Customer (Name, Address, PostCode) OUTPUT INSERTED.Id VALUES (@Name, @Address, @PostCode)";
            var parameters = new
            {
                Name = string.IsNullOrEmpty(item.Name) ? (object)DBNull.Value : item.Name,
                Address = string.IsNullOrEmpty(item.Address) ? (object)DBNull.Value : item.Address,
                PostCode = string.IsNullOrEmpty(item.PostCode) ? (object)DBNull.Value : item.PostCode
            };

            item.Id = connection.ExecuteScalar<int>(sql, parameters);
        }
    }
}
