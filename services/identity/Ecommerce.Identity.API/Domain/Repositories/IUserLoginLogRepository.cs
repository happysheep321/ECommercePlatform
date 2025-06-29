using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IUserLoginLogRepository
    {
        Task AddAsync(UserLoginLog log);
        Task<IEnumerable<UserLoginLog>> GetLogsByUserIdAsync(Guid userId);
    }
}
