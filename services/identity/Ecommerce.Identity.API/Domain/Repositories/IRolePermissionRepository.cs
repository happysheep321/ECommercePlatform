using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId);
        Task AddAsync(RolePermission permission);
        void Remove(RolePermission permission);
    }
}
