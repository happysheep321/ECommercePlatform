using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public record RolePermission(Guid RoleId, Guid PermissionId);
}
