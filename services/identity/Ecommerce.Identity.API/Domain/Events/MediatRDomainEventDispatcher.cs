using System.Threading.Tasks;
using MediatR;
using Ecommerce.SharedKernel.Events;
using Ecommerce.SharedKernel.Interfaces;

namespace Ecommerce.Identity.API.Domain.Events
{
    public class MediatRDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator mediator;

        public MediatRDomainEventDispatcher(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            await mediator.Publish(domainEvent);
        }
    }
}