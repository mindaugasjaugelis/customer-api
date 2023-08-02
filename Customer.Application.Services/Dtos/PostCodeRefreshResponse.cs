using System.Text.Json.Serialization;

namespace Customer.Application.Services.Dtos
{
    public class PostCodeRefreshResponse
    {
        [JsonPropertyName("success")]
        public bool? Success { get; set; }

        [JsonPropertyName("data")]
        public List<PostCodeRefreshResponseDataItem> Data { get; set; } = new List<PostCodeRefreshResponseDataItem>();
    }
}
