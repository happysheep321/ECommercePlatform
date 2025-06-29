using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
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

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await this.context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == name);
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
    }
}
