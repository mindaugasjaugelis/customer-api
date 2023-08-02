using Customer.Domain.Models;

namespace Customer.Application.Abstractions.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? PostCode { get; set; } = string.Empty;

        public CustomerDto(CustomerEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Address = entity.Address;
            PostCode = entity.PostCode;
        }
    }
}