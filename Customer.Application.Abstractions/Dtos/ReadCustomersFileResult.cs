using System.Collections.Generic;

namespace Customer.Application.Abstractions.Dtos
{
    public class ReadCustomersFileResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<CustomerImportDto> Customers { get; set; }

        public ReadCustomersFileResult(bool success)
        {
            Success = success;
        }
    }
}
