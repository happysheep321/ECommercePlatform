using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Identity.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;
        private readonly ILogger<RoleController> logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            this.roleService = roleService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<Guid>>> CreateAsync([FromBody] CreateRoleCommand command)
        {
            var id = await roleService.CreateRoleAsync(command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<Guid>.Ok(id, "Created"));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateRoleCommand command)
        {
            await roleService.UpdateRoleAsync(command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "Updated"));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteRoleCommand command)
        {
            await roleService.DeleteRoleAsync(command);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "Deleted"));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<RoleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<List<RoleDto>>>> GetAllAsync()
        {
            var roles = await roleService.GetAllRolesAsync();
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<List<RoleDto>>.Ok(roles));
        }

        [HttpGet("{roleId}")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<RoleDto?>>> GetByIdAsync(Guid roleId)
        {
            var role = await roleService.GetRoleByIdAsync(new GetRoleByIdCommand { RoleId = roleId });
            if (role == null) return NotFound(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Fail("NOT_FOUND", "角色不存在"));
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<RoleDto?>.Ok(role));
        }

        [HttpGet("{roleId}/permissions")]
        [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ECommerce.SharedKernel.DTOs.ApiResponse<IReadOnlyList<PermissionDto>>>> GetPermissionsAsync(Guid roleId)
        {
            var perms = await roleService.GetPermissionsByRoleIdAsync(roleId);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<IReadOnlyList<PermissionDto>>.Ok(perms));
        }

        [HttpPost("{roleId}/permissions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AssignPermissionsAsync(Guid roleId, [FromBody] List<Guid> permissionIds)
        {
            await roleService.AssignPermissionsToRoleAsync(roleId, permissionIds);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "Assigned"));
        }

        [HttpDelete("{roleId}/permissions/{permissionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemovePermissionAsync(Guid roleId, Guid permissionId)
        {
            await roleService.RemovePermissionFromRoleAsync(roleId, permissionId);
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "Removed"));
        }

        [HttpPost("{roleId}/enable")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> EnableAsync(Guid roleId)
        {
            await roleService.EnableAsync(new EnableRoleCommand { RoleId = roleId });
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "Enabled"));
        }

        [HttpPost("{roleId}/disable")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DisableAsync(Guid roleId)
        {
            await roleService.DisableAsync(new DisableRoleCommand { RoleId = roleId });
            return Ok(ECommerce.SharedKernel.DTOs.ApiResponse<string>.Ok("OK", "Disabled"));
        }
    }
}
