using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using ECommerce.Identity.API.Domain.Aggregates.RoleAggregate;

namespace ECommerce.Identity.API.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Role>> GetByIdsAsync(IEnumerable<Guid> roleIds);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task AddAsync(Role role);
        void Update(Role role);
        void Delete(Role role);

        // 处理权限
        Task<IReadOnlyList<Permission>> GetPermissionsByRoleIdAsync(Guid roleId);
    }
}
