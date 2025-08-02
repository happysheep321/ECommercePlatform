using System.Threading.Tasks;
using Ecommerce.SharedKernel.Events;

namespace ECommerce.SharedKernel.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}