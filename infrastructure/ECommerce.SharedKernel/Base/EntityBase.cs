using System;
using System.Collections.Generic;
using ECommerce.SharedKernel.Events;
using ECommerce.SharedKernel.Interfaces;

namespace ECommerce.SharedKernel.Base
{
    public abstract class EntityBase<TId> : Entity<TId>, IAggregateRoot
    {
        private List<IDomainEvent>? domainEvents;
        public IReadOnlyCollection<IDomainEvent>? DomainEvents => domainEvents?.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            domainEvents ??= new List<IDomainEvent>();
            domainEvents.Add(domainEvent);
        }

        protected void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            domainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            domainEvents?.Clear();
        }
    }
}