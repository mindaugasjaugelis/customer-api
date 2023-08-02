using Customer.Domain.Models;

namespace Customer.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<CustomerEntity>> GetCustomersAsync();
        Task InsertCustomersAsync(List<CustomerEntity> list);
        Task<CustomerEntity> UpdateCustomerAsync(CustomerEntity toUpdate);
    }
}
