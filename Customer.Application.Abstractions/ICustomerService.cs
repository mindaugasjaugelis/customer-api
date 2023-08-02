using Customer.Application.Abstractions.Dtos;

namespace Customer.Application.Abstractions
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetCustomersAsync();
        Task<ImportCustomersFromFileResult> ImportCustomers();
        Task RefreshPostCodeFromPostLt();
    }
}
