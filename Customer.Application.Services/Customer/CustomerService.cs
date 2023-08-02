using System.Text.Json;
using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Configuration;
using Customer.Application.Abstractions.Dtos;
using Customer.Application.Services.Dtos;
using Customer.Domain.Models;
using Customer.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Customer.WebApi.Services
{
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
            List<CustomerEntity> customerEntities = await _customerRepository.GetCustomersAsync();
            return customerEntities
                .Select(x => new CustomerDto(x))
                .ToList();
        }

        public async Task<ImportCustomersFromFileResult> ImportCustomers()
        {
            ReadCustomersFileResult readCustomersFileResponse = await _dataReader.Read("data/customers.json");
            if (!readCustomersFileResponse.Success)
            {
                return new ImportCustomersFromFileResult(false)
                {
                    ErrorMessage = readCustomersFileResponse.ErrorMessage
                };
            }

            IEnumerable<CustomerEntity> dbCustomers = await _customerRepository.GetCustomersAsync();
            List<CustomerImportDto> uqCustomers = AvoidDuplicates(readCustomersFileResponse.Customers, dbCustomers.ToList());
            List<CustomerEntity> customerEntitiesToImport = uqCustomers
                .Select(x => x.ToCustomerEntity())
                .ToList();
            await _customerRepository.InsertCustomersAsync(customerEntitiesToImport);
            return new ImportCustomersFromFileResult(true);
        }

        public async Task RefreshPostCodeFromPostLt()
        {
            List<CustomerEntity> dbCustomers = await _customerRepository.GetCustomersAsync();
            foreach (CustomerEntity customer in dbCustomers)
            {
                await RefreshPostCodeFromPostLt(customer);
            }
        }

        private static List<CustomerImportDto> AvoidDuplicates(List<CustomerImportDto> toImport, List<CustomerEntity> currentInDb)
        {
            if (currentInDb == null || currentInDb.Count == 0)
            {
                return toImport;
            }

            return toImport
                .Where(x => !HasDuplicate(x, currentInDb))
                .ToList();
        }

        private static bool HasDuplicate(CustomerImportDto toImport, List<CustomerEntity> currentInDb)
        {
            return currentInDb.Where(x => HasSameName(x.Name, toImport.Name)).Any();
        }

        private static bool HasSameName(string nameToImport, string nameInDb)
        {
            return nameToImport == nameInDb;
        }

        private async Task RefreshPostCodeFromPostLt(CustomerEntity customer)
        {
            string freshCutomerPostCode = await GetAddressPostCode(customer.Address);
            if (!string.IsNullOrEmpty(freshCutomerPostCode) && customer.PostCode != freshCutomerPostCode)
            {
                customer.PostCode = freshCutomerPostCode;
                await _customerRepository.UpdateCustomerAsync(customer);
            }
        }

        private async Task<string> GetAddressPostCode(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return string.Empty;
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
                    var result = JsonSerializer.Deserialize<PostCodeRefreshResponse>(jsonResult);
                    if (result?.Success == true && result.Data?.Count > 0)
                    {
                        return result.Data.First().PostCode ?? string.Empty;
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
