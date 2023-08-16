using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Configuration;
using Customer.Application.Abstractions.Dtos.PostLtClient;
using Microsoft.Extensions.Options;

namespace Customer.WebApi.Services
{
    public class PostLtClient : IPostLtClient
    {
        readonly PostLtOptions _postLtOptions;

        public PostLtClient(IOptionsSnapshot<PostLtOptions> postLtOptions)
        {
            if (postLtOptions == null)
            {
                throw new ArgumentNullException(nameof(postLtOptions));
            }

            _postLtOptions = postLtOptions.Value;
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

            var jsonResult = await SendRequest(GetPostLtApiRequestQuery(address));
            if (string.IsNullOrEmpty(jsonResult))
            {
                return new PostLtResponse
                {
                    Success = false,
                    Data = new List<PostLtResponseDataItem>()
                };
            }

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

        private async Task<string> SendRequest(string query)
        {
            var postLtApiUrl = _postLtOptions.BaseUrl;
            using var httpClient = new HttpClient();
            try
            {
                var url = $"{postLtApiUrl}/{query}";
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                return null;
            }
        }

        private string GetPostLtApiRequestQuery(string address)
        {
            var postLtKey = _postLtOptions.Key;
            return $"?term={GetPostLtApiRequestTerm(address)}&key={postLtKey}";
        }

        private static string GetPostLtApiRequestTerm(string address)
        {
            return address.Trim().Replace(" ", "+");
        }
    }
}
