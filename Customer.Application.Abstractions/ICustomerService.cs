using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Application.Abstractions.Dtos;

namespace Customer.Application.Abstractions
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetCustomersAsync();
        Task<ImportCustomersFromFileResult> ImportFromFile();
        Task<bool> RefreshPostCodeFromPostLt();
    }
}
