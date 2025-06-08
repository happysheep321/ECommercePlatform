using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class UserAddressRepository:IUserAddressRepository
    {
        private readonly IdentityDbContext context;
        private readonly IUserRepository userRepository;

        public UserAddressRepository(IdentityDbContext context, IUserRepository userRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(Guid userId)
        {
            return await context.UserAddresses
                .Where(ua=>ua.UserId==userId)
                .ToListAsync();
        }

        public async Task AddAsync(UserAddress userAddress)
        {
            var user = await userRepository.GetByIdAsync(userAddress.UserId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");
            user.AddAddress(userAddress);
        }

        public async Task UpdateAsync(UserAddress userAddress)
        {
            var user= await userRepository.GetByIdAsync(userAddress.UserId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");
            user.UpdateAddress(userAddress);
        }

        public async Task DeleteAsync(UserAddress userAddress)
        {
            var user = await userRepository.GetByIdAsync(userAddress.UserId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");
            user.RemoveAddress(userAddress);
        }
    }
}
