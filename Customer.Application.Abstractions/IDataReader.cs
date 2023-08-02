using Customer.Application.Abstractions.Dtos;

namespace Customer.Application.Abstractions
{
    public interface IDataReader
    {
        Task<ReadCustomersFileResult> Read(string path);
    }
}
