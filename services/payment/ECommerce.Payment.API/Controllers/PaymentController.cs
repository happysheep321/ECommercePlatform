using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Payment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController:ControllerBase
    {
        [HttpGet]
        public IActionResult PaymentTest()
        {
            return this.Ok();
        }
    }
}
