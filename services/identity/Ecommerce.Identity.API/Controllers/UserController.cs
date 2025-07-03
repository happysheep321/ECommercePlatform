using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // ---------- 用户资料 ----------

        [HttpGet("{userId}/profile")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserProfileDto>> GetProfileAsync(Guid userId)
        {
            try
            {
                var profile = await userService.GetProfileAsync(userId);
                if (profile == null)
                    return NotFound("用户不存在");

                return Ok(profile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取用户资料失败，UserId: {UserId}", userId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPut("{userId}/profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfileAsync(Guid userId, [FromBody] UpdateUserProfileCommand command)
        {
            try
            {
                await userService.UpdateProfileAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "更新用户资料失败，UserId: {UserId}", userId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        // ---------- 地址管理 ----------

        [HttpPost("{userId}/address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAddressAsync(Guid userId, [FromBody] AddUserAddressCommand command)
        {
            try
            {
                await userService.AddAddressAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "添加地址失败，UserId: {UserId}", userId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPut("{userId}/address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAddressAsync(Guid userId, [FromBody] UpdateUserAddressCommand command)
        {
            try
            {
                await userService.UpdateAddressAsync(userId, command);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "更新地址失败，UserId: {UserId}", userId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpDelete("{userId}/address/{addressId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveAddressAsync(Guid userId, Guid addressId)
        {
            try
            {
                await userService.RemoveAddressAsync(userId, addressId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除地址失败，UserId: {UserId}, AddressId: {AddressId}", userId, addressId);
                return StatusCode(500, "服务器内部错误");
            }
        }

        // ---------- 角色管理 ----------

        [HttpPost("{userId}/role/assign")]
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

        [HttpDelete("{userId}/role/remove")]
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
