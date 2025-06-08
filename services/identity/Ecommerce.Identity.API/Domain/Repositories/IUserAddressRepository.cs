using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IUserAddressRepository
    {
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserAddress address);
        Task UpdateAsync(UserAddress address);
        Task DeleteAsync(UserAddress address);
    }
}
