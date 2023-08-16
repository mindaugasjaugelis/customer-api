using Customer.Infrastructure.DataBase;
using Customer.Application.Abstractions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.WebApi.StartupExtensions
{
    public static class OptionsExtensions
    {
        public static void AddCustomerOptions(this IServiceCollection services)
        {
            services.AddOptions<PostLtOptions>().BindConfiguration(PostLtOptions.ConfigurationPath);
            services.AddOptions<ConnectionStringOptions>().BindConfiguration(string.Empty);
        }
    }
}
