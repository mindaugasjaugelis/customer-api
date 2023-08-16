namespace Customer.Application.Services.Dtos
{
    public class ReadFileContentResult
    {
        public bool Success { get; set; }
        public string Content { get; set; }
        public string ErrorMessage { get; set; }

        public ReadFileContentResult(bool success)
        {
            Success = success;
        }
    }
}
