using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
        [HttpGet]
        public IActionResult ProductTest()
        {
            return this.Ok();
        }
    }
}
