using System.Threading.Tasks;
using Ecommerce.SharedKernel.Events;

namespace Ecommerce.SharedKernel.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}