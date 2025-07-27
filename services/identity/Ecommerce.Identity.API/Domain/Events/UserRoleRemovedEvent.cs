using System;
using Ecommerce.SharedKernel.Events;

namespace Ecommerce.Identity.API.Domain.Events
{
    public class UserRoleRemovedEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public Guid RoleId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserRoleRemovedEvent(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}