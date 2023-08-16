using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Customer.Application.Abstractions.Dtos.PostLtClient
{
    public class PostLtResponse
    {
        [JsonPropertyName("success")]
        public bool? Success { get; set; }

        [JsonPropertyName("data")]
        public List<PostLtResponseDataItem> Data { get; set; }
    }
}
