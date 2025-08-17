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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;
        private readonly ILogger<RoleController> logger;
        private readonly IMediator mediator;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger, IMediator mediator)
        {
            this.roleService = roleService;
            this.logger = logger;
            this.mediator = mediator;
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        /// <param name="command">创建角色指令</param>
        /// <returns>新创建的角色ID</returns>
        /// <remarks>
        /// 创建一个新的角色，需要提供角色名称和描述
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "name": "Editor",
        ///   "displayName": "编辑员",
        ///   "description": "内容编辑权限"
        /// }
        /// ```
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateAsync([FromBody] CreateRoleCommand command)
        {
            var id = await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.Ok(id, "角色创建成功"));
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="command">更新角色指令</param>
        /// <returns>更新操作结果</returns>
        /// <remarks>
        /// 更新指定角色的基本信息，包括名称、显示名称和描述
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "id": "role-id-here",
        ///   "name": "Editor",
        ///   "displayName": "高级编辑员",
        ///   "description": "高级内容编辑权限"
        /// }
        /// ```
        /// </remarks>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateAsync([FromBody] UpdateRoleCommand command)
        {
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "角色更新成功"));
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="command">删除角色指令</param>
        /// <returns>删除操作结果</returns>
        /// <remarks>
        /// 删除指定的角色，如果角色已被用户使用则无法删除
        /// 
        /// 示例请求:
        /// ```json
        /// {
        ///   "roleId": "role-id-here"
        /// }
        /// ```
        /// </remarks>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync([FromBody] DeleteRoleCommand command)
        {
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "角色删除成功"));
        }

        /// <summary>
        /// 获取所有角色列表
        /// </summary>
        /// <returns>角色列表</returns>
        /// <remarks>
        /// 获取系统中所有可用的角色列表，包括已启用和已禁用的角色
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAllAsync()
        {
            var command = new GetAllRolesCommand();
            var roles = await mediator.Send(command);
            return Ok(ApiResponse<List<RoleDto>>.Ok(roles));
        }

        /// <summary>
        /// 根据ID获取角色详情
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>角色详情</returns>
        /// <remarks>
        /// 根据角色ID获取角色的详细信息，包括权限列表
        /// </remarks>
        [HttpGet("{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto?>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<RoleDto?>>> GetByIdAsync(Guid roleId)
        {
            var command = new GetRoleByIdCommand { RoleId = roleId };
            var role = await mediator.Send(command);
            if (role == null) return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "角色不存在"));
            return Ok(ApiResponse<RoleDto?>.Ok(role));
        }

        /// <summary>
        /// 获取角色的权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限列表</returns>
        /// <remarks>
        /// 获取指定角色拥有的所有权限列表
        /// </remarks>
        [HttpGet("{roleId}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PermissionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionDto>>>> GetPermissionsAsync(Guid roleId)
        {
            var command = new GetPermissionsCommand { RoleId = roleId };
            var perms = await mediator.Send(command);
            return Ok(ApiResponse<IReadOnlyList<PermissionDto>>.Ok(perms));
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID列表</param>
        /// <returns>分配操作结果</returns>
        /// <remarks>
        /// 为指定角色分配多个权限
        /// 
        /// 示例请求:
        /// ```json
        /// ["permission-id-1", "permission-id-2", "permission-id-3"]
        /// ```
        /// </remarks>
        [HttpPost("{roleId}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> AssignPermissionsAsync(Guid roleId, [FromBody] List<Guid> permissionIds)
        {
            var command = new AssignPermissionsCommand { RoleId = roleId, PermissionIds = permissionIds };
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "权限分配成功"));
        }

        /// <summary>
        /// 移除角色的指定权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionId">权限ID</param>
        /// <returns>移除操作结果</returns>
        /// <remarks>
        /// 从指定角色中移除指定的权限
        /// </remarks>
        [HttpDelete("{roleId}/permissions/{permissionId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> RemovePermissionAsync(Guid roleId, Guid permissionId)
        {
            var command = new RemovePermissionCommand { RoleId = roleId, PermissionId = permissionId };
            await mediator.Send(command);
            return Ok(ApiResponse<string>.Ok("OK", "权限移除成功"));
        }

        /// <summary>
        /// 启用角色
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>启用操作结果</returns>
        /// <remarks>
        /// 启用指定的角色，使其可以被分配给用户
        /// </remarks>
        [HttpPost("{roleId}/enable")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> EnableAsync(Guid roleId)
        {
            await mediator.Send(new EnableRoleCommand { RoleId = roleId });
            return Ok(ApiResponse<string>.Ok("OK", "角色启用成功"));
        }

        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>禁用操作结果</returns>
        /// <remarks>
        /// 禁用指定的角色，使其无法被分配给新用户
        /// </remarks>
        [HttpPost("{roleId}/disable")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DisableAsync(Guid roleId)
        {
            await mediator.Send(new DisableRoleCommand { RoleId = roleId });
            return Ok(ApiResponse<string>.Ok("OK", "角色禁用成功"));
        }
    }
}
