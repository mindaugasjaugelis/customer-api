namespace Customer.Application.Abstractions.Dtos
{
    public class ImportCustomersFromFileResult
    {
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;

        public ImportCustomersFromFileResult(bool success)
        {
            Success = success;
        }
    }
}
