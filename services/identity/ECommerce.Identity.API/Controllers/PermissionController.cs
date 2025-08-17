using ECommerce.Identity.API.Application.Commands;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace ECommerce.Identity.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService permissionService;
        private readonly ILogger<PermissionController> logger;
        private readonly IMediator mediator;

        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger, IMediator mediator)
        {
            this.permissionService = permissionService;
            this.logger = logger;
            this.mediator = mediator;
        }

        /// <summary>
        /// 获取所有权限列表
        /// </summary>
        /// <returns>权限列表</returns>
        /// <remarks>
        /// 获取系统中所有可用的权限列表，用于角色权限分配
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetAllAsync()
        {
            var command = new GetAllPermissionsCommand();
            var permissions = await mediator.Send(command);
            return Ok(ApiResponse<List<PermissionDto>>.Ok(permissions));
        }

        /// <summary>
        /// 根据ID获取权限详情
        /// </summary>
        /// <param name="permissionId">权限ID</param>
        /// <returns>权限详情</returns>
        /// <remarks>
        /// 根据权限ID获取权限的详细信息
        /// </remarks>
        [HttpGet("{permissionId}")]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto?>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PermissionDto?>>> GetByIdAsync(Guid permissionId)
        {
            var permission = await permissionService.GetPermissionByIdAsync(permissionId);
            if (permission == null) return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "权限不存在"));
            return Ok(ApiResponse<PermissionDto?>.Ok(permission));
        }

        /// <summary>
        /// 检查用户是否有指定权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionCode">权限代码</param>
        /// <returns>权限检查结果</returns>
        /// <remarks>
        /// 检查指定用户是否拥有指定的权限
        /// </remarks>
        [HttpGet("check/{userId}/{permissionCode}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckPermissionAsync(Guid userId, string permissionCode)
        {
            var hasPermission = await permissionService.HasPermissionAsync(userId, permissionCode);
            return Ok(ApiResponse<bool>.Ok(hasPermission));
        }
    }
}
