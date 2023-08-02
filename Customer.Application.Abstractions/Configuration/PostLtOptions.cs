namespace Customer.Application.Abstractions.Configuration
{
    public class PostLtOptions
    {
        public const string ConfigurationPath = "PostLt";
        public string BaseUrl { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}
