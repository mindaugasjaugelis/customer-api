using System.Collections.Generic;
using Customer.Application.Abstractions.Dtos;

namespace Customer.Application.Services.Dtos
{
    public class DeserializeFileContentToCustomersListResult
    {
        public bool Success { get; set; }
        public List<CustomerImportDto> Customers { get; set; }
        public string ErrorMessage { get; set; }

        public DeserializeFileContentToCustomersListResult(bool success)
        {
            Success = success;
        }
    }
}
