using System.Text.Json.Serialization;

namespace Customer.WebApi.Models
{
    public class PostLtRequestResult
    {
        [JsonPropertyName("success")]
        public bool? Success { get; set; }

        [JsonPropertyName("data")]
        public List<PostLtRequestResultDataItem>? Data { get; set; }
    }

    public class PostLtRequestResultDataItem 
    {
        [JsonPropertyName("post_code")]
        public string? PostCode { get; set; }
    }
}
