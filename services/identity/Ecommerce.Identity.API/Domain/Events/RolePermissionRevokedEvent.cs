using Ecommerce.SharedKernel.Events;
using System;

namespace Ecommerce.Identity.API.Domain.Events
{
    public class RolePermissionRevokedEvent : DomainEvent
    {
        public Guid RoleId { get; }
        public Guid PermissionId { get; }
        public RolePermissionRevokedEvent(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
} 