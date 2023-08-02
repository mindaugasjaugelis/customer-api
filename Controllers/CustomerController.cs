using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Dtos;
using Customer.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet(Name = "GetCustomers")]
        public async Task<ActionResult> GetCustomers()
        {
            List<CustomerDto> customerDtosList = await _customerService.GetCustomersAsync();
            IEnumerable<CustomerModel> customerModelsList = customerDtosList.Select(x => new CustomerModel(x));
            var result = new
            {
                Success = true,
                CustomerList = customerModelsList
            };

            return new JsonResult(result);
        }

        [HttpPost(Name = "ImportCustomers")]
        public async Task<ActionResult> ImportCustomers()
        {
            ImportCustomersFromFileResult importCustomersFromFileResult = await _customerService.ImportCustomers();
            return new JsonResult(importCustomersFromFileResult);
        }

        [HttpPost(Name = "RefreshPostCode")]
        public async Task<ActionResult> RefreshPostCode()
        {
            await _customerService.RefreshPostCodeFromPostLt();
            var successResult = new
            {
                Success = true
            };

            return new JsonResult(successResult);
        }
    }
}