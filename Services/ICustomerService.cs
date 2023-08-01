using System.Text.Json;
using Customer.WebApi.Models;
using Customer.WebApi.Models.Customer;
using Customer.WebApi.Models.Resources;
using Microsoft.Extensions.Options;

namespace Customer.WebApi.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetCustomersAsync();
        Task ImportCustomers();
        Task RefreshPostCodeFromPostLt();
    }

    public class CustomerService : ICustomerService
    {
        readonly PostLtOptions _postLtOptions;
        private readonly IDataReader _dataReader;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            IOptionsSnapshot<PostLtOptions> postLtOptions,
            IDataReader dataReader,
            ICustomerRepository customerRepository)
        {
            if (postLtOptions == null)
            {
                throw new ArgumentNullException(nameof(postLtOptions));
            }

            _postLtOptions = postLtOptions.Value;
            _dataReader = dataReader;
            _customerRepository = customerRepository;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            return await _customerRepository.GetCustomersAsync();
        }

        public async Task ImportCustomers()
        {
            var customersToImport = await _dataReader.Read("data/customers.json");
            if (!customersToImport.Any())
            {
                return;
            }

            var dbCustomers = await _customerRepository.GetCustomersAsync();
            var newCustomers = AvoidDuplicates(customersToImport, dbCustomers);
            await _customerRepository.InsertCustomersAsync(newCustomers);
        }

        public async Task RefreshPostCodeFromPostLt()
        {
            var dbCustomers = await _customerRepository.GetCustomersAsync();
            foreach(var customer in dbCustomers)
            {
                await RefreshPostCodeFromPostLt(customer);
            }
        }

        private static List<CustomerDto> AvoidDuplicates(List<CustomerDto> toImport, List<CustomerDto>? currentInDb)
        {
            if (currentInDb == null || currentInDb.Count == 0)
            {
                return toImport;
            }

            return toImport
                .Where(x => !HasDuplicate(x, currentInDb))
                .ToList();
        }

        private static bool HasDuplicate(CustomerDto toImport, List<CustomerDto> currentInDb)
        {
            return currentInDb.Where(x => HasSameName(x.Name, toImport.Name)).Any();
        }

        private static bool HasSameName(string nameToImport, string nameInDb)
        {
            return nameToImport == nameInDb;
        }

        private async Task RefreshPostCodeFromPostLt(CustomerDto customer)
        {
            var newPostCode = await GetAddressPostCode(customer.Address);
            if (newPostCode != null && customer.PostCode != newPostCode)
            {
                customer.PostCode = newPostCode;
                await _customerRepository.UpdateCustomerAsync(customer);
            }
        }

        private async Task<string?> GetAddressPostCode(string? address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            var postLtApiUrl = _postLtOptions.BaseUrl;
            var postLtKey = _postLtOptions.Key;
            using var httpClient = new HttpClient();
            try
            {
                var url = $"{postLtApiUrl}/?term={GetPostLtApiRequestTerm(address)}&key={postLtKey}";
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<PostLtRequestResult>(jsonResult);
                    if (result?.Success == true && result.Data?.Count > 0)
                    {
                        return result.Data.First().PostCode;
                    }
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
            }

            return null;
        }

        private static string GetPostLtApiRequestTerm(string address)
        {
            return address.Trim().Replace(" ", "+");
        }
    }
}
