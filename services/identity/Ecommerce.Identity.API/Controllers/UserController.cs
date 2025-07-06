using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Identity.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : throw new UnauthorizedAccessException("用户未认证");
        }

        // ---------- 用户资料 ----------

        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserProfileDto>> GetProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await userService.GetProfileAsync(userId);
                if (profile == null)
                    return NotFound("用户不存在");

                return Ok(profile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取用户资料失败");
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserProfileCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                await userService.UpdateProfileAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "更新用户资料失败");
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePasswordPasswordAsync([FromBody] ChangePasswordCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                await userService.ChangePasswordAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "修改密码失败");
                return StatusCode(500, "服务器内部错误");
            }
        }

        // ---------- 地址管理 ----------

        [HttpPost("address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAddressAsync([FromBody] AddUserAddressCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                await userService.AddAddressAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "添加地址失败");
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPut("address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateUserAddressCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                await userService.UpdateAddressAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "更新地址失败");
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpDelete("address/{addressId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveAddressAsync(Guid addressId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await userService.RemoveAddressAsync(userId, addressId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除地址失败，AddressId: {AddressId}", addressId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        // ---------- 角色管理 ----------

        [HttpPost("role/assign")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignRoleAsync(Guid userId, [FromQuery] Guid roleId)
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

        [HttpDelete("role/remove")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveRoleAsync(Guid userId, [FromQuery] Guid roleId)
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
