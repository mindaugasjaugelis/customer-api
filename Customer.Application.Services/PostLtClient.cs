using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Configuration;
using Customer.Application.Abstractions.Dtos.PostLtClient;
using Customer.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Customer.WebApi.Services
{
    public class PostLtClient : IPostLtClient
    {
        readonly PostLtOptions _postLtOptions;
        private readonly IDataReader _dataReader;
        private readonly ICustomerRepository _customerRepository;

        public PostLtClient(
            IOptionsSnapshot<PostLtOptions> postLtOptions,
            IDataReader dataReader,
            ICustomerRepository customerRepository)
        {
            if (postLtOptions == null)
            {
                throw new ArgumentNullException(nameof(postLtOptions));
            }

            _postLtOptions = postLtOptions.Value;
            _dataReader = dataReader;
            _customerRepository = customerRepository;
        }

        public async Task<PostLtResponse> GetByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return new PostLtResponse
                {
                    Success = false,
                    Data = new List<PostLtResponseDataItem>()
                };
            }

            var postLtApiUrl = _postLtOptions.BaseUrl;
            var postLtKey = _postLtOptions.Key;
            using var httpClient = new HttpClient();
            try
            {
                var url = $"{postLtApiUrl}/?term={GetPostLtApiRequestTerm(address)}&key={postLtKey}";
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<PostLtResponse>(jsonResult);
                    if (result?.Success == true && result.Data?.Count > 0)
                    {
                        return result;
                    }
                    else
                    {
                        return new PostLtResponse
                        {
                            Success = false,
                            Data = new List<PostLtResponseDataItem>()
                        };
                    }
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
            }

            return null;
        }

        private static string GetPostLtApiRequestTerm(string address)
        {
            return address.Trim().Replace(" ", "+");
        }
    }
}
