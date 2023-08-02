using Customer.Domain.Models;

namespace Customer.Domain.Repositories
{
    public interface ILogRepository
    {
        Task InsertLogAsync(CustomerEntity item, Models.Action action);
    }
}
