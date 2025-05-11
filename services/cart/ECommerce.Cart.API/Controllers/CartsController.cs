using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController:ControllerBase
    {
        [HttpGet]
        public IActionResult CartTest()
        {
            return this.Ok();
        }
    }
}
