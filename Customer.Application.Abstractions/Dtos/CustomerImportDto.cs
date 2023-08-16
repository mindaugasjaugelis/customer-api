using Customer.Domain.Models;

namespace Customer.Application.Abstractions.Dtos
{
    public class CustomerImportDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }

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
