using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class PermissionRepository:IPermissionRepository
    {
        private readonly IdentityDbContext context;

        public PermissionRepository(IdentityDbContext context)
        {
            this.context = context;
        }

        public async Task<Permission?> GetByIdAsync(Guid id)
        {
            return await this.context.Permissions.FirstOrDefaultAsync(p=>p.Id == id);
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await this.context.Permissions
                .AsNoTracking()
                .OrderBy(p=>p.Name)
                .ToListAsync();
        }
    }
}
