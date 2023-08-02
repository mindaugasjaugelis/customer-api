using Customer.Domain.Repositories;
using Customer.Infrastructure.DataBase.Repositories;

namespace Customer.WebApi.StartupExtensions
{
    public static class RepositoriesExtensions
    {
        public static void AddCustomerRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
        }
    }
}
