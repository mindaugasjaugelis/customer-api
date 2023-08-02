using System.Text.Json.Serialization;

namespace Customer.Application.Services.Dtos
{
    public class PostCodeRefreshResponseDataItem
    {
        [JsonPropertyName("post_code")]
        public string? PostCode { get; set; }
    }
}
