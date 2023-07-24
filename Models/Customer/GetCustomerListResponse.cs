namespace Customer.WebApi.Models.Customer
{
    public class GetCustomerListResponse
    {
        public bool Success { get; set; } = true;
        public IEnumerable<CustomerDto>? List { get; set; }
    }
}
