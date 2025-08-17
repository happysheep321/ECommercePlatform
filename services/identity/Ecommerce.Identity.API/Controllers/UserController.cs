using ECommerce.Identity.API.Application.Commands;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Application.Services;
using ECommerce.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using FluentValidation;

namespace ECommerce.Identity.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
            public class UserController : ControllerBase
        {
            private readonly IUserService userService;
            private readonly ILogger<UserController> logger;
            private readonly IMediator mediator;
            private readonly IAvatarService avatarService;

            public UserController(IUserService userService, ILogger<UserController> logger, IMediator mediator, IAvatarService avatarService)
            {
                this.userService = userService;
                this.logger = logger;
                this.mediator = mediator;
                this.avatarService = avatarService;
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
            var command = new GetUserProfileCommand();
            var profile = await mediator.Send(command);
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
            await mediator.Send(command);
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
            await mediator.Send(command);
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
            await mediator.Send(command);
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
            await mediator.Send(new ActivateUserCommand { UserId = userId });
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
            var command = new AssignRoleCommand { UserId = userId, RoleId = roleId };
            await mediator.Send(command);
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
            var command = new RemoveRoleCommand { UserId = userId, RoleId = roleId };
            await mediator.Send(command);
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
            await mediator.Send(new BanUserCommand { UserId = userId, Reason = reason });
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
            await mediator.Send(new FreezeUserCommand { UserId = userId, Reason = reason });
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
        [HttpDelete("admin/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(Guid userId)
        {
            await mediator.Send(new DeleteUserCommand { UserId = userId });
            return Ok(ApiResponse<string>.Ok("OK", "用户删除成功"));
        }

        // ===== 管理员功能 =====

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        /// <remarks>
        /// 管理员操作：获取系统中所有用户的列表，包含用户状态、角色等管理信息
        /// 不包含详细地址信息，如需详细信息请调用单个用户详情接口
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        [ProducesResponseType(typeof(ApiResponse<List<UserListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<UserListDto>>>> GetAllUsersAsync()
        {
            var command = new GetAllUsersCommand();
            var users = await mediator.Send(command);
            return Ok(ApiResponse<List<UserListDto>>.Ok(users));
        }

        /// <summary>
        /// 根据ID获取用户详情
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户详情</returns>
        /// <remarks>
        /// 管理员操作：根据用户ID获取指定用户的详细信息
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto?>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserProfileDto?>>> GetUserByIdAsync(Guid userId)
        {
            var command = new GetUserByIdCommand { UserId = userId };
            var user = await mediator.Send(command);
            if (user == null) return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "用户不存在"));
            return Ok(ApiResponse<UserProfileDto?>.Ok(user));
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
            await mediator.Send(command);
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
            var command = new RemoveAddressCommand { AddressId = addressId };
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "地址删除成功"));
        }

        /// <summary>
        /// 上传用户头像
        /// </summary>
        /// <param name="file">头像文件</param>
        /// <returns>上传结果</returns>
        /// <remarks>
        /// 上传用户头像文件，支持jpg、png、gif格式，文件大小不超过2MB
        /// </remarks>
        [HttpPost("avatar")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> UploadAvatarAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("INVALID_FILE", "请选择要上传的文件"));

            // 验证文件类型
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(ApiResponse<string>.Fail("INVALID_FILE_TYPE", "只支持jpg、png、gif格式的图片"));

            // 验证文件大小（2MB）
            if (file.Length > 2 * 1024 * 1024)
                return BadRequest(ApiResponse<string>.Fail("FILE_TOO_LARGE", "文件大小不能超过2MB"));

            try
            {
                var userId = GetCurrentUserId();
                var avatarUrl = await avatarService.UploadAvatarAsync(file, userId);
                
                // 更新用户头像URL
                var updateCommand = new UpdateUserProfileCommand
                {
                    AvatarUrl = avatarUrl
                };
                await mediator.Send(updateCommand);

                return Ok(ApiResponse<string>.Ok(avatarUrl, "头像上传成功"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "用户 {UserId} 上传头像失败", GetCurrentUserId());
                return StatusCode(500, ApiResponse<string>.Fail("UPLOAD_FAILED", $"头像上传失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>头像文件</returns>
        [HttpGet("avatar/{userId}")]
        [AllowAnonymous]  // 允许匿名访问头像
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvatarAsync(Guid userId)
        {
            try
            {
                // 直接调用Service，因为这是公开的头像访问
                var profile = await userService.GetProfileAsync(userId);
                
                if (profile == null)
                {
                    return NotFound();
                }

                // 获取用户头像（UserProfile已经处理了默认头像URL）
                var avatarBytes = await avatarService.GetAvatarAsync(profile.AvatarUrl);
                if (avatarBytes != null)
                {
                    var contentType = GetContentType(Path.GetExtension(profile.AvatarUrl));
                    return File(avatarBytes, contentType);
                }

                // 如果获取失败，返回默认头像
                var defaultAvatarBytes = await avatarService.GetDefaultAvatarAsync();
                if (defaultAvatarBytes != null)
                {
                    return File(defaultAvatarBytes, "image/png");
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取用户 {UserId} 头像失败", userId);
                return NotFound();
            }
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
