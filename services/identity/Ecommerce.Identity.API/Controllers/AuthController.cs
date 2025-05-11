using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        [HttpGet]
        public IActionResult UserTest()
        {
            return this.Ok();
        }
    }
}
