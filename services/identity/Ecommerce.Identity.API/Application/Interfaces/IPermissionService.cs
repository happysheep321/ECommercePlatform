using ECommerce.Identity.API.Application.DTOs;
using System.Threading.Tasks;

namespace ECommerce.Identity.API.Application.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// 获取所有权限（固定表）
        /// </summary>
        Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync();

        /// <summary>
        /// 根据权限ID获取权限
        /// </summary>
        Task<PermissionDto?> GetPermissionByIdAsync(Guid permissionId);

        /// <summary>
        /// 根据角色ID获取权限列表
        /// </summary>
        Task<IReadOnlyList<PermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId);

        /// <summary>
        /// 检查用户是否有某个权限
        /// </summary>
        Task<bool> HasPermissionAsync(Guid userId, string permissionCode);
    }
}
