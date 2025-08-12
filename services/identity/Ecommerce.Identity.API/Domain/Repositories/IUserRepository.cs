using ECommerce.Identity.API.Domain.Aggregates.UserAggregate;

namespace ECommerce.Identity.API.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
    }
}
