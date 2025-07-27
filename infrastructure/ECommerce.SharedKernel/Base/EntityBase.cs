using System;
using System.Collections.Generic;
using Ecommerce.SharedKernel.Events;
using Ecommerce.SharedKernel.Interfaces;

namespace Ecommerce.SharedKernel.Base
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