using ECommerce.SharedKernel.Events;

namespace ECommerce.Identity.API.Domain.Events
{
    public class RolePermissionGrantedEvent : IDomainEvent
    {
        public Guid RoleId { get; }
        public Guid PermissionId { get; }

        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public RolePermissionGrantedEvent(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
} 