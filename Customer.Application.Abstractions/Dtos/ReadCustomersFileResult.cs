namespace Customer.Application.Abstractions.Dtos
{
    public class ReadCustomersFileResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<CustomerImportDto> Customers { get; set; } = new List<CustomerImportDto>();

        public ReadCustomersFileResult(bool success)
        {
            Success = success;
        }
    }
}
