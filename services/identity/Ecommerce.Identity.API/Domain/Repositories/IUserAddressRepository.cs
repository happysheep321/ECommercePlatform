using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IUserAddressRepository
    {
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserAddress address);
        void Update(UserAddress address);
        void Delete(UserAddress address);
    }
}
