using ECommerce.Identity.API.Domain.Aggregates.UserAggregate;
using ECommerce.Identity.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Identity.API.Infrastructure.Repositories
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
                        .Include(u => u.UserRoles)
                        .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await this.context.Users
                        .Include(u => u.Addresses)
                        .Include(u => u.UserRoles)
                        .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await this.context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await this.context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await this.context.Users
                .Include(u => u.Addresses)
                .Include(u => u.UserRoles)
                .Include(u => u.Profile)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await this.context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            //标记主聚合根 User 实体为修改
            this.context.Users.Update(user);

            //处理 Addresses 子集合
            var trackedAddresses =context.UserAddresses
                                .Where(a => a.UserId == user.Id)
                                .ToList();

            foreach (var address in user.Addresses)
            {
                var tracked = trackedAddresses.FirstOrDefault(a => a.Id == address.Id);
                var entry=context.Entry(address);

                if (tracked == null)
                {
                    context.UserAddresses.Add(address);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }

            var removedAddresses = trackedAddresses
                                .Where(a => !user.Addresses.Any(ua => ua.Id == a.Id))
                                .ToList();

            foreach (var removed in removedAddresses)
            {
                context.UserAddresses.Remove(removed);
            }
        }

        public void Delete(User user)
        {
            this.context.Users.Remove(user);
        }
    }
}
