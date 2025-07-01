using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly IdentityDbContext dbContext;

        public UserAddressRepository(IdentityDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(UserAddress address)
        {
            await dbContext.UserAddresses.AddAsync(address);
        }

        public void Update(UserAddress address)
        {
            dbContext.UserAddresses.Update(address);
        }

        public void Remove(UserAddress address)
        {
            dbContext.UserAddresses.Remove(address);
        }

        public async Task<UserAddress?> GetByIdAsync(Guid addressId)
        {
            return await dbContext.UserAddresses.FirstOrDefaultAsync(a => a.Id == addressId);
        }

        public async Task<IEnumerable<UserAddress>> GetAddressesByUserIdAsync(Guid userId)
        {
            return await dbContext.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }
    }
}
