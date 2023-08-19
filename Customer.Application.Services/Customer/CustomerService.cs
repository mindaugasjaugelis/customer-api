using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Dtos;
using Customer.Domain.Models;
using Customer.Domain.Repositories;

namespace Customer.WebApi.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly IDataReader _dataReader;
        private readonly IPostLtClient _postLtClient;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            IDataReader dataReader,
            IPostLtClient postLtClient,
            ICustomerRepository customerRepository)
        {
            _dataReader = dataReader;
            _postLtClient = postLtClient;
            _customerRepository = customerRepository;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            var customerEntities = await _customerRepository.GetCustomersAsync();
            return customerEntities
                .Select(x => new CustomerDto(x))
                .ToList();
        }

        public async Task<ImportCustomersFromFileResult> ImportFromFile()
        {
            var readCustomersFileResponse = await _dataReader.Read("data/customers.json");
            if (!readCustomersFileResponse.Success)
            {
                return new ImportCustomersFromFileResult(false)
                {
                    ErrorMessage = readCustomersFileResponse.ErrorMessage
                };
            }

            var dbCustomers = await _customerRepository.GetCustomersAsync();
            var uqCustomers = AvoidDuplicates(readCustomersFileResponse.Customers, dbCustomers.ToList());
            var customerEntitiesToImport = uqCustomers
                .Select(x => x.ToCustomerEntity())
                .ToList();
            await _customerRepository.InsertCustomersAsync(customerEntitiesToImport);
            return new ImportCustomersFromFileResult(true);
        }

        public async Task<bool> RefreshPostCodeFromPostLt()
        {
            var dbCustomers = await _customerRepository.GetCustomersAsync();
            var success = true;
            foreach (var customer in dbCustomers)
            {
                var refreshPostCodeFromPostLtSuccess = await RefreshPostCodeFromPostLt(customer);
                if (!refreshPostCodeFromPostLtSuccess)
                {
                    success = false;
                }
            }

            //TODO: return error message for each refresh iteration if error occur
            return success;
        }

        private static List<CustomerImportDto> AvoidDuplicates(List<CustomerImportDto> toImport, List<CustomerEntity> currentInDb)
        {
            if (currentInDb.Count == 0)
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

        private async Task<bool> RefreshPostCodeFromPostLt(CustomerEntity customer)
        {
            var postLtResponse = await _postLtClient.GetByAddress(customer.Address);
            if (!postLtResponse.Success.HasValue || !postLtResponse.Success.Value)
            {
                return false;
            }

            var postCodeReceived = postLtResponse.Data.FirstOrDefault()?.PostCode;
            if (string.IsNullOrEmpty(postCodeReceived))
            {
                return false;
            }

            if (customer.PostCode != postCodeReceived)
            {
                customer.PostCode = postCodeReceived;
                await _customerRepository.UpdateCustomerAsync(customer);
            }

            return true;
        }
    }
}
