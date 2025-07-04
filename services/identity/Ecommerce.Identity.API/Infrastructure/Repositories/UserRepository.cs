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

            //处理 UserLoginLogs 子集合
            var trackedLoginLogs = context.UserLoginLogs
                                    .Where(log => log.UserId == user.Id)
                                    .ToList();

            var newLoginLogs = user.LoginLogs
                .Where(log => !trackedLoginLogs.Any(tlog => tlog.Id == log.Id))
                .ToList();

            context.UserLoginLogs.AddRange(newLoginLogs);

            //处理 UserRoles 子集合
            var trackedUserRoles = context.UserRoles
                                    .Where(r => r.UserId == user.Id)
                                    .ToList();

            foreach (var userRole in user.UserRoles)
            {
                if (!trackedUserRoles.Any(r => r.RoleId == userRole.RoleId))
                {
                    context.UserRoles.Add(userRole);
                }
            }

            var removedRoles = trackedUserRoles
                                .Where(existing => !user.UserRoles.Any(r => r.RoleId == existing.RoleId))
                                .ToList();

            foreach (var removed in removedRoles)
            {
                context.UserRoles.Remove(removed);
            }
        }

        public void Delete(User user)
        {
            this.context.Users.Remove(user);
        }
    }
}
