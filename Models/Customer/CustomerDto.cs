namespace Customer.WebApi.Models.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address{ get; set; } = string.Empty;
        public string? PostCode { get; set; } = string.Empty;
    }
}
