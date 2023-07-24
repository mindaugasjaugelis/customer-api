using Customer.WebApi.Models.Customer;
using Dapper;

namespace Customer.WebApi.Services
{
    public interface ICustomerRepository
    {
        List<CustomerDto> GetList();
        void Import(List<CustomerDto> list);
        void Update(CustomerDto toUpdate);
    }

    public class CustomerRepository: ICustomerRepository
    {
        private readonly IConnectionProvider _connectionProvider;

        public CustomerRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public List<CustomerDto> GetList()
        {
            string query = "SELECT * FROM dbo.Customer";
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();
            return connection.Query<CustomerDto>(query).ToList();
        }

        public void Import(List<CustomerDto> list)
        {
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();

            string sql = "INSERT INTO dbo.Customer (Name, Address, PostCode) VALUES (@Name, @Address, @PostCode)";
            foreach (var item in list)
            {
                var parameters = new
                {
                    Name = string.IsNullOrEmpty(item.Name) ? (object)DBNull.Value : item.Name,
                    Address = string.IsNullOrEmpty(item.Address) ? (object)DBNull.Value : item.Address,
                    PostCode = string.IsNullOrEmpty(item.PostCode) ? (object)DBNull.Value : item.PostCode
                };

                connection.Execute(sql, parameters);
            }
        }

        public void Update(CustomerDto toUpdate)
        {
            using var connection = _connectionProvider.GetSqlConnection();
            connection.Open();

            string sql = "UPDATE dbo.Customer set PostCode = @PostCode WHERE Id = @Id";
            connection.Execute(sql, toUpdate);
        }
    }
}
