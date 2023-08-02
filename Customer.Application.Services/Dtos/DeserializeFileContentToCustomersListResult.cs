using Customer.Application.Abstractions.Dtos;

namespace Customer.Application.Services.Dtos
{
    public class DeserializeFileContentToCustomersListResult
    {
        public bool Success { get; set; } = true;
        public List<CustomerImportDto> Customers { get; set; } = new List<CustomerImportDto>();
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
