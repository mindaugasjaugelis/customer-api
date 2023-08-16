using System.Text.Json.Serialization;

namespace Customer.Application.Abstractions.Dtos.PostLtClient
{
    public class PostLtResponseDataItem
    {
        [JsonPropertyName("post_code")]
        public string PostCode { get; set; }
    }
}
