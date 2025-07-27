using Ecommerce.SharedKernel.Events;
using System;

namespace Ecommerce.Identity.API.Domain.Events
{
    public class UserLoggedInEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string IP { get; }
        public string Device { get; }
        public string Location { get; }

        public UserLoggedInEvent(Guid userId, string ip, string device, string location)
        {
            UserId = userId;
            IP = ip;
            Device = device;
            Location = location;
        }
    }
} 