using Customer.Domain.Models;

namespace Customer.Application.Abstractions.Dtos
{
    public class CustomerImportDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;

        public CustomerEntity ToCustomerEntity()
        {
            return new CustomerEntity
            {
                Name = Name,
                Address = Address,
                PostCode = PostCode
            };
        }
    }
}
