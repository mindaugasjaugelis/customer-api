using Customer.WebApi.Models.Customer;

namespace Customer.WebApi.Services
{
    public interface ICustomerDataReader
    {
        List<CustomerDto> Read(string path);
    }

    public class CustomerDataReader : ICustomerDataReader
    {
        private readonly IConnectionProvider _connectionProvider;

        public CustomerDataReader(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public List<CustomerDto> Read(string path)
        {
            var jsonData = File.ReadAllText(path);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDto>>(jsonData)
                ?? new List<CustomerDto>();
        }
    }
}
