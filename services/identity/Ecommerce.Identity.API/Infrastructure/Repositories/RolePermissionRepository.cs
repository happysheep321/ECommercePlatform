using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class RolePermissionRepository:IRolePermissionRepository
    {
        private readonly IdentityDbContext context;

        public RolePermissionRepository(IdentityDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            return await context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task AddAsync(RolePermission permission)
        {
            await context.RolePermissions.AddAsync(permission);
        }

        public void Remove(RolePermission rolePermission)
        {
            context.RolePermissions.Remove(rolePermission);
        }
    }
}
