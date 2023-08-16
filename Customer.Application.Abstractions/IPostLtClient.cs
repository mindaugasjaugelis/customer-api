using System.Threading.Tasks;
using Customer.Application.Abstractions.Dtos.PostLtClient;

namespace Customer.Application.Abstractions
{
    public interface IPostLtClient
    {
        Task<PostLtResponse> GetByAddress(string address);
    }
}
