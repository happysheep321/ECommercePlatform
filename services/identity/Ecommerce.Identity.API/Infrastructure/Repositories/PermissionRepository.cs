using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using ECommerce.Identity.API.Domain.Repositories;
using ECommerce.SharedKernel.Constants;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Identity.API.Infrastructure.Repositories
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

        public async Task<List<Permission>> GetDefaultPermissionsAsync()
        {
            var defaultPermissionNames = new List<string>()
            {
                PermissionNames.UserView,
                PermissionNames.RoleView,
                PermissionNames.ProductView
            };

            return await this.context.Permissions.Where(p=>defaultPermissionNames.Contains(p.Name!)).ToListAsync();
        }
    }
}
