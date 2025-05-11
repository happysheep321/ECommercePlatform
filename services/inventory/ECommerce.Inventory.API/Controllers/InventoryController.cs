using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController:ControllerBase
    {
        [HttpGet]
        public IActionResult InventoryItemTest()
        {
            return this.Ok();
        }
    }
}
