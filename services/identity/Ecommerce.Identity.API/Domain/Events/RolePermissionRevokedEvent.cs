using Ecommerce.SharedKernel.Events;

namespace Ecommerce.Identity.API.Domain.Events
{
    public class RolePermissionRevokedEvent : IDomainEvent
    {
        public Guid RoleId { get; }
        public Guid PermissionId { get; }

        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public RolePermissionRevokedEvent(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
} 