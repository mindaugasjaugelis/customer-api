using Customer.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository, ICustomerService customerService)
        {
            _customerRepository = customerRepository;
            _customerService = customerService;
        }

        [HttpGet(Name = "Get")]
        public ActionResult Get()
        {
            return new JsonResult(new { Success = true, List = _customerRepository.GetList() });
        }

        [HttpPost(Name = "Import")]
        public ActionResult Import()
        {
            _customerService.Import();
            return new JsonResult(new { Success = true });
        }

        [HttpPost(Name = "RefreshPostCode")]
        public async Task<ActionResult> RefreshPostCode()
        {
            await _customerService.RefreshPostCodeFromPostLt();
            return new JsonResult(new { Success = true });
        }
    }
}