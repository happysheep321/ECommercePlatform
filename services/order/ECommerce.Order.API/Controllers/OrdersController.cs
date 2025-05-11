using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController:ControllerBase
    {
        [HttpGet]
        public IActionResult OrderTest()
        {
            return this.Ok();
        }
    }
}
