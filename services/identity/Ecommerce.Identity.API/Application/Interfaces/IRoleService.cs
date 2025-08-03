using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;

namespace Ecommerce.Identity.API.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Guid> CreateRoleAsync(CreateRoleCommand command);

        Task UpdateRoleAsync(UpdateRoleCommand command);

        Task DeleteRoleAsync(DeleteRoleCommand command);

        Task<List<RoleDto>> GetAllRolesAsync();

        Task<RoleDto?> GetRoleByIdAsync(GetRoleByIdCommand command);

        // 角色-权限绑定
        Task AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds);
        Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
        Task<IReadOnlyList<PermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId);
    }
}
