using Customer.Domain.Models;

namespace Customer.Application.Abstractions.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }

        public CustomerDto(CustomerEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Address = entity.Address;
            PostCode = entity.PostCode;
        }
    }
}