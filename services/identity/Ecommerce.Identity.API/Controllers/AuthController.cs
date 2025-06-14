using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult UserTest()
        {
            return this.Ok();
        }
    }
}
