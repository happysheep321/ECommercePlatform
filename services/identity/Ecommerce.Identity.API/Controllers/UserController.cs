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
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<UserProfileDto>>> GetProfileAsync()
        {
            var userId = GetCurrentUserId();
            var profile = await userService.GetProfileAsync(userId);
            if (profile == null)
                return NotFound(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Fail("NOT_FOUND", "用户不存在"));
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<UserProfileDto>.Ok(profile));
        }

        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserProfileCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.UpdateProfileAsync(userId, command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "更新成功"));
        }

        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePasswordPasswordAsync([FromBody] ChangePasswordCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.ChangePasswordAsync(userId, command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "修改成功"));
        }

        // ---------- 地址管理 ----------

        [HttpPost("address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAddressAsync([FromBody] AddUserAddressCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.AddAddressAsync(userId, command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "添加成功"));
        }

        // ---------- 用户状态管理 ----------
        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/activate")]
        public async Task<IActionResult> ActivateAsync(Guid userId)
        {
            await userService.ActivateAsync(new ActivateUserCommand { UserId = userId });
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "已激活"));
        }

        // ---------- 用户角色 ----------
        [Authorize(Roles = "Admin")]
        [HttpPost("role/assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignRoleAsync([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            await userService.AssignRoleAsync(userId, roleId);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "分配成功"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("role/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveRoleAsync([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            await userService.RemoveRoleAsync(userId, roleId);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "移除成功"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/ban")]
        public async Task<IActionResult> BanAsync(Guid userId, [FromBody] string reason)
        {
            await userService.BanAsync(new BanUserCommand { UserId = userId, Reason = reason });
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "已封禁"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/freeze")]
        public async Task<IActionResult> FreezeAsync(Guid userId, [FromBody] string reason)
        {
            await userService.FreezeAsync(new FreezeUserCommand { UserId = userId, Reason = reason });
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "已冻结"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync(Guid userId)
        {
            await userService.DeleteAsync(new DeleteUserCommand { UserId = userId });
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "已注销"));
        }

        [HttpPut("address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateUserAddressCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.UpdateAddressAsync(userId, command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "更新成功"));
        }

        [HttpDelete("address/{addressId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveAddressAsync(Guid addressId)
        {
            var userId = GetCurrentUserId();
            await userService.RemoveAddressAsync(userId, addressId);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "删除成功"));
        }
    }
}
