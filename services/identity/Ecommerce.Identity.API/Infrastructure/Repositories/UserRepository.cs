using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext context;

        public UserRepository(IdentityDbContext context)
        {
            this.context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var user=await this.context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            var user=await this.context.Users.FirstOrDefaultAsync(u=>u.UserName == userName);
            return user;
        }

        public async Task AddAsync(User user)
        {
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            this.context.Users.Update(user);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            this.context.Users.Remove(user);
            await this.context.SaveChangesAsync();
        }
    }
}
