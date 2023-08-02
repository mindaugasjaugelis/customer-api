namespace Customer.Application.Services.Dtos
{
    public class ReadFileContentResult
    {
        public bool Success { get; set; } = true;
        public string Content { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
