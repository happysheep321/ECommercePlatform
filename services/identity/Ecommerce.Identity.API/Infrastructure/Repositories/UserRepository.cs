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
            return await this.context.Users
                        .Include(u=>u.Addresses)
                        .Include(u => u.LoginLogs)
                        .Include(u=>u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await this.context.Users
                        .Include(u => u.Addresses)
                        .Include(u => u.LoginLogs)
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await this.context.Users.AnyAsync(u => u.PhoneNumber == phone);
        }

        public async Task AddAsync(User user)
        {
            await this.context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            this.context.Users.Update(user);
        }

        public void Delete(User user)
        {
            this.context.Users.Remove(user);
        }
    }
}
