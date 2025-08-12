using ECommerce.Identity.API.Application.Commands;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Identity.API.Controllers
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

        /// <summary>
        /// 获取当前用户资料
        /// </summary>
        /// <returns>用户资料信息</returns>
        /// <remarks>
        /// 获取当前登录用户的详细资料信息，包括基本信息、联系方式等
        /// 需要用户已登录认证
        /// </remarks>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfileAsync()
        {
            var userId = GetCurrentUserId();
            var profile = await userService.GetProfileAsync(userId);
            if (profile == null)
                return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "用户不存在"));
            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="command">用户资料更新指令</param>
        /// <returns>更新操作结果</returns>
        /// <remarks>
        /// 更新当前登录用户的资料信息，包括姓名、性别、生日等基本信息
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "firstName": "张",
        ///   "lastName": "三",
        ///   "gender": 1,
        ///   "birthDate": "1990-01-01",
        ///   "phone": "13800138000"
        /// }
        /// ```
        /// </remarks>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateProfileAsync([FromBody] UpdateUserProfileCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.UpdateProfileAsync(userId, command);
            return Ok(ApiResponse<string>.Ok("OK", "更新成功"));
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="command">密码修改指令</param>
        /// <returns>密码修改结果</returns>
        /// <remarks>
        /// 修改当前登录用户的密码，需要提供旧密码和新密码
        /// 新密码必须符合密码强度要求
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "oldPassword": "oldPassword123",
        ///   "newPassword": "newPassword123"
        /// }
        /// ```
        /// </remarks>
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> ChangePasswordPasswordAsync([FromBody] ChangePasswordCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.ChangePasswordAsync(userId, command);
            return Ok(ApiResponse<string>.Ok("OK", "修改成功"));
        }

        // ---------- 地址管理 ----------

        /// <summary>
        /// 添加用户地址
        /// </summary>
        /// <param name="command">添加地址指令</param>
        /// <returns>添加操作结果</returns>
        /// <remarks>
        /// 为当前登录用户添加新的收货地址
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "recipientName": "张三",
        ///   "phone": "13800138000",
        ///   "province": "广东省",
        ///   "city": "深圳市",
        ///   "district": "南山区",
        ///   "detailAddress": "科技园路1号",
        ///   "isDefault": true
        /// }
        /// ```
        /// </remarks>
        [HttpPost("address")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> AddAddressAsync([FromBody] AddUserAddressCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.AddAddressAsync(userId, command);
            return Ok(ApiResponse<string>.Ok("OK", "地址添加成功"));
        }

        // ---------- 用户状态管理 ----------
        
        /// <summary>
        /// 激活用户账户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>激活操作结果</returns>
        /// <remarks>
        /// 管理员操作：激活指定的用户账户，使其可以正常使用系统
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/activate")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> ActivateAsync(Guid userId)
        {
            await userService.ActivateAsync(new ActivateUserCommand { UserId = userId });
            return Ok(ApiResponse<string>.Ok("OK", "用户激活成功"));
        }

        // ---------- 用户角色 ----------
        
        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>分配操作结果</returns>
        /// <remarks>
        /// 管理员操作：为指定用户分配指定的角色
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost("role/assign")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> AssignRoleAsync([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            await userService.AssignRoleAsync(userId, roleId);
            return Ok(ApiResponse<string>.Ok("OK", "角色分配成功"));
        }

        /// <summary>
        /// 移除用户角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>移除操作结果</returns>
        /// <remarks>
        /// 管理员操作：从指定用户中移除指定的角色
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpDelete("role/remove")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> RemoveRoleAsync([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            await userService.RemoveRoleAsync(userId, roleId);
            return Ok(ApiResponse<string>.Ok("OK", "角色移除成功"));
        }

        /// <summary>
        /// 封禁用户账户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reason">封禁原因</param>
        /// <returns>封禁操作结果</returns>
        /// <remarks>
        /// 管理员操作：封禁指定的用户账户，用户将无法登录系统
        /// 
        /// 示例请求:
        /// ```json
        /// "违反社区规定"
        /// ```
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/ban")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> BanAsync(Guid userId, [FromBody] string reason)
        {
            await userService.BanAsync(new BanUserCommand { UserId = userId, Reason = reason });
            return Ok(ApiResponse<string>.Ok("OK", "用户封禁成功"));
        }

        /// <summary>
        /// 冻结用户账户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reason">冻结原因</param>
        /// <returns>冻结操作结果</returns>
        /// <remarks>
        /// 管理员操作：冻结指定的用户账户，用户暂时无法使用系统功能
        /// 
        /// 示例请求:
        /// ```json
        /// "账户异常，需要审核"
        /// ```
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/freeze")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> FreezeAsync(Guid userId, [FromBody] string reason)
        {
            await userService.FreezeAsync(new FreezeUserCommand { UserId = userId, Reason = reason });
            return Ok(ApiResponse<string>.Ok("OK", "用户冻结成功"));
        }

        /// <summary>
        /// 删除用户账户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>删除操作结果</returns>
        /// <remarks>
        /// 管理员操作：永久删除指定的用户账户，此操作不可恢复
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(Guid userId)
        {
            await userService.DeleteAsync(new DeleteUserCommand { UserId = userId });
            return Ok(ApiResponse<string>.Ok("OK", "用户删除成功"));
        }

        /// <summary>
        /// 更新用户地址
        /// </summary>
        /// <param name="command">更新地址指令</param>
        /// <returns>更新操作结果</returns>
        /// <remarks>
        /// 更新当前登录用户的指定地址信息
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "addressId": "address-id-here",
        ///   "recipientName": "张三",
        ///   "phone": "13800138000",
        ///   "province": "广东省",
        ///   "city": "深圳市",
        ///   "district": "南山区",
        ///   "detailAddress": "科技园路1号",
        ///   "isDefault": true
        /// }
        /// ```
        /// </remarks>
        [HttpPut("address")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateAddressAsync([FromBody] UpdateUserAddressCommand command)
        {
            var userId = GetCurrentUserId();
            await userService.UpdateAddressAsync(userId, command);
            return Ok(ApiResponse<string>.Ok("OK", "地址更新成功"));
        }

        /// <summary>
        /// 删除用户地址
        /// </summary>
        /// <param name="addressId">地址ID</param>
        /// <returns>删除操作结果</returns>
        /// <remarks>
        /// 删除当前登录用户的指定地址
        /// </remarks>
        [HttpDelete("address/{addressId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> RemoveAddressAsync(Guid addressId)
        {
            var userId = GetCurrentUserId();
            await userService.RemoveAddressAsync(userId, addressId);
            return Ok(ApiResponse<string>.Ok("OK", "地址删除成功"));
        }
    }
}
