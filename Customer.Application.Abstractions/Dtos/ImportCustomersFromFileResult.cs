namespace Customer.Application.Abstractions.Dtos
{
    public class ImportCustomersFromFileResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public ImportCustomersFromFileResult(bool success)
        {
            Success = success;
        }
    }
}
