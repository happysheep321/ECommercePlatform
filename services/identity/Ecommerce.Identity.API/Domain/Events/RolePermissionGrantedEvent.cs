using Ecommerce.SharedKernel.Events;
using System;

namespace Ecommerce.Identity.API.Domain.Events
{
    public class RolePermissionGrantedEvent : DomainEvent
    {
        public Guid RoleId { get; }
        public Guid PermissionId { get; }
        public RolePermissionGrantedEvent(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
} 