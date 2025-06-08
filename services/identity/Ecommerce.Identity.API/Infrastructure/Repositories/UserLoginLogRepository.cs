using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class UserLoginLogRepository : IUserLoginLogRepository
    {
        private readonly IdentityDbContext context;
        private readonly IUserRepository userRepository;

        public UserLoginLogRepository(IdentityDbContext context, IUserRepository userRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<UserLoginLog>> GetByUserIdAsync(Guid userId)
        {
            return await context.UserLoginLogs
                .Where(ul => ul.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(UserLoginLog userLoginLog)
        {
            var user = await userRepository.GetByIdAsync(userLoginLog.UserId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");
            user.AddLoginLog(userLoginLog);
        }
    }
}
