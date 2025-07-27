using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Ecommerce.SharedKernel.Interfaces;
using Ecommerce.SharedKernel.Events;
using Ecommerce.Identity.API.Domain.Events;

namespace Ecommerce.Identity.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(UserRoleAssignedEvent).Assembly);
            });
            return services;
        }
    }
}