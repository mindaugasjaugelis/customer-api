using System.Text.Json;
using Customer.WebApi.Models;
using Customer.WebApi.Models.Customer;
using Customer.WebApi.Models.Resources;
using Microsoft.Extensions.Options;

namespace Customer.WebApi.Services
{
    public interface ICustomerService
    {
        void Import();
        void RefreshPostCodeFromPostLt();
    }

    public class CustomerService : ICustomerService
    {
        readonly PostLtOptions _postLtOptions;
        private readonly ICustomerDataReader _customerDataReader;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            IOptionsSnapshot<PostLtOptions> postLtOptions,
            ICustomerDataReader customerDataReader,
            ICustomerRepository customerRepository)
        {
            if (postLtOptions == null)
            {
                throw new ArgumentNullException(nameof(postLtOptions));
            }

            _postLtOptions = postLtOptions.Value;
            _customerDataReader = customerDataReader;
            _customerRepository = customerRepository;
        }

        public void Import()
        {
            var customers = _customerDataReader.Read("data/customers.json");
            if (!customers.Any()) {
                return;
            }

            var dbCustomers = _customerRepository.GetList();
            var newCustomers = AvoidDuplicates(customers, dbCustomers);
            _customerRepository.Insert(newCustomers);
        }

        public void RefreshPostCodeFromPostLt()
        {
            var dbCustomers = _customerRepository.GetList();
            foreach(var customer in dbCustomers)
            {
                RefreshPostCodeFromPostLt(customer);
            }
        }

        private static List<CustomerDto> AvoidDuplicates(List<CustomerDto> toImport, List<CustomerDto>? curentInDb)
        {
            if (curentInDb == null || curentInDb.Count == 0) {
                return toImport;
            }

            return toImport.Where(x => !HasDuplicate(x, curentInDb))
                .ToList();
        }

        private static bool HasDuplicate(CustomerDto toImport, List<CustomerDto> curentInDb)
        {
            return curentInDb.Where(x => x.Name == toImport.Name).Any();
        }

        private void RefreshPostCodeFromPostLt(CustomerDto customer)
        {
            var newPostCode = GetAddressPostCode(customer.Address).Result;
            if (newPostCode != null && customer.PostCode != newPostCode)
            {
                customer.PostCode = newPostCode;
                _customerRepository.Update(customer);
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
                var url = $"{postLtApiUrl}/?term={GetTerm(address)}&key={postLtKey}";
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

        private string GetTerm(string address)
        {
            return address.Trim().Replace(" ", "+");
        }
    }
}
