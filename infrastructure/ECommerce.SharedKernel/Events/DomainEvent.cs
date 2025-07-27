namespace Ecommerce.SharedKernel.Events
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
    }
} 