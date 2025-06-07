using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;

namespace Ecommerce.Identity.API.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(Guid id);
        Task<IEnumerable<Permission>> GetAllAsync();
    }
}
