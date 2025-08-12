using System.Threading.Tasks;
using ECommerce.SharedKernel.Events;

namespace ECommerce.SharedKernel.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}