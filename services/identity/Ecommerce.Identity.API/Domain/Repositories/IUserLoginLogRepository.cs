using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IUserLoginLogRepository
    {
        Task<IEnumerable<UserLoginLog>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserLoginLog log); 
    }
}
