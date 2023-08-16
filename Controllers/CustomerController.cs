using System.Threading.Tasks;
using Customer.Application.Abstractions;
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
            var customerDtosList = await _customerService.GetCustomersAsync();
            var result = new
            {
                Success = true,
                CustomerList = customerDtosList
            };

            return new JsonResult(result);
        }

        [HttpPost(Name = "ImportCustomers")]
        public async Task<ActionResult> ImportCustomers()
        {
            var importCustomersFromFileResult = await _customerService.ImportFromFile();
            return new JsonResult(importCustomersFromFileResult);
        }

        [HttpPost(Name = "RefreshPostCode")]
        public async Task<ActionResult> RefreshPostCode()
        {
            var refreshPostCodeFromPostLtSuccess = await _customerService.RefreshPostCodeFromPostLt();
            var successResult = new
            {
                Success = refreshPostCodeFromPostLtSuccess
            };

            return new JsonResult(successResult);
        }
    }
}