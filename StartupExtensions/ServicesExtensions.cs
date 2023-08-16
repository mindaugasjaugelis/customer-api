using Customer.Application.Abstractions;
using Customer.Application.Services.DataReader;
using Customer.Infrastructure.DataBase.Abstractions;
using Customer.Infrastructure.DataBase;
using Customer.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Customer.WebApi.Services.Customer;

namespace Customer.WebApi.StartupExtensions
{
    public static class ServicesExtensions
    {
        public static void AddCustomerServices(this IServiceCollection services)
        {
            services.AddSingleton<ISqlConnectionProvider, SqlConnectionProvider>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDataReader, FileDataReader>();
            services.AddScoped<IPostLtClient, PostLtClient>();
        }
    }
}
