using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IUserAddressRepository
    {
        Task AddAsync(UserAddress address);
        void Update(UserAddress address);
        void Remove(UserAddress address);
        Task<UserAddress?> GetByIdAsync(Guid addressId);
        Task<IEnumerable<UserAddress>> GetAddressesByUserIdAsync(Guid userId);
    }
}
