using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetRolesByUserIdAsync(Guid userId);
        Task AddAsync(UserRole userRole);
        void Remove(UserRole userRole);
    }
}
