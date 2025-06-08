using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class UserRoleRepository:IUserRoleRepository
    {
        private readonly IdentityDbContext context;

        public UserRoleRepository(IdentityDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<UserRole>> GetRolesByUserIdAsync(Guid userId)
        {
            return await context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(UserRole userRole)
        {
            await context.UserRoles.AddAsync(userRole);
        }

        public void Remove(UserRole userRole)
        {
            context.UserRoles.Remove(userRole);
        }
    }
}
