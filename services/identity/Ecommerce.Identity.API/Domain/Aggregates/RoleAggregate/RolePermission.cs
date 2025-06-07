using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public required Role Role { get; set; }
        public required Permission Permission { get; set; }
    }
}
