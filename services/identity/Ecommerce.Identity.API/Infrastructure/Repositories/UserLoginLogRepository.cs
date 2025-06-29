using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class UserLoginLogRepository : IUserLoginLogRepository
    {
        private readonly IdentityDbContext dbContext;

        public UserLoginLogRepository(IdentityDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(UserLoginLog log)
        {
            await dbContext.UserLoginLogs.AddAsync(log);
        }

        public async Task<IEnumerable<UserLoginLog>> GetLogsByUserIdAsync(Guid userId)
        {
            return await dbContext.UserLoginLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.LoginTime)
                .ToListAsync();
        }
    }
}
