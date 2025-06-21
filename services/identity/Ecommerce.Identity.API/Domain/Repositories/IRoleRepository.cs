using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task AddAsync(Role role);
        void Update(Role role);
        void Delete(Role role);
    }
}
