using Customer.Application.Abstractions.Dtos;

namespace Customer.WebApi.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? PostCode { get; set; } = string.Empty;

        public CustomerModel(CustomerDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Address = dto.Address;
            PostCode = dto.PostCode;
        }
    }
}
