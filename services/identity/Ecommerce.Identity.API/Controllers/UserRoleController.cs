using Ecommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Identity.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserRoleController> logger;

        public UserRoleController(IUserService userService, ILogger<UserRoleController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        /// <summary>
        /// 给用户分配角色（仅管理员）
        /// </summary>
        [HttpPost("assign")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignRoleAsync([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            try
            {
                await userService.AssignRoleAsync(userId, roleId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "分配角色失败，UserId: {UserId}, RoleId: {RoleId}", userId, roleId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        /// <summary>
        /// 移除用户角色（仅管理员）
        /// </summary>
        [HttpDelete("remove")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveRoleAsync([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            try
            {
                await userService.RemoveRoleAsync(userId, roleId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "移除角色失败，UserId: {UserId}, RoleId: {RoleId}", userId, roleId);
                return StatusCode(500, "服务器内部错误");
            }
        }
    }
}