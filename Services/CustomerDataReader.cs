using Customer.WebApi.Models.Customer;

namespace Customer.WebApi.Services
{
    public interface IDataReader
    {
        Task<List<CustomerDto>> Read(string path);
    }

    public class CustomerDataReader : IDataReader
    {
        public async Task<List<CustomerDto>> Read(string path)
        {
            var jsonData = await File.ReadAllTextAsync(path);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerDto>>(jsonData)
                ?? new List<CustomerDto>();
        }
    }
}
