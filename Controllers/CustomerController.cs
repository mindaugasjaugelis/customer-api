using Customer.WebApi.Services;
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
            var customerList = await _customerService.GetCustomersAsync();
            var result = new
            {
                Success = true,
                customerList
            };

            return new JsonResult(result);
        }

        [HttpPost(Name = "ImportCustomers")]
        public async Task<ActionResult> ImportCustomers()
        {
            await _customerService.ImportCustomers();
            var successResult = new
            {
                Success = true
            };

            return new JsonResult(successResult);
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