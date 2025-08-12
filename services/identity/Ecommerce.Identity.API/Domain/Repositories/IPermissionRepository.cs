using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;

namespace ECommerce.Identity.API.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(Guid id);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<List<Permission>> GetDefaultPermissionsAsync();
    }
}
