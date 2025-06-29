using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
