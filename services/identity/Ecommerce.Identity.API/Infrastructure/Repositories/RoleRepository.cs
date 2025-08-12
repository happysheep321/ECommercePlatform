using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using ECommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using ECommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Identity.API.Infrastructure.Repositories
{
    public class RoleRepository:IRoleRepository
    {
        private readonly IdentityDbContext context;

        public RoleRepository(IdentityDbContext context)
        {
            this.context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await this.context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyList<Role>> GetByIdsAsync(IEnumerable<Guid> roleIds)
        {
            return await this.context.Roles
                .Where(r=>roleIds.Contains(r.Id))
                .ToListAsync();
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await this.context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await this.context.Roles
                .AsNoTracking()
                .OrderBy(r=>r.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Role role)
        {
            await this.context.Roles.AddAsync(role);
        }

        public void Update(Role role)
        {
            this.context.Roles.Update(role);
        }

        public void Delete(Role role)
        {
            this.context.Roles.Remove(role);
        }

        public async Task<IReadOnlyList<Permission>> GetPermissionsByRoleIdAsync(Guid roleId)
        {
            var permissionIds = await context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            return await context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string Name)
        {
            return await this.context.Roles.AnyAsync(r => r.Name == Name);
        }
    }
}
