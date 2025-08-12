using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ECommerce.Identity.API.Domain.Events;

namespace ECommerce.Identity.API.Application.EventHandlers
{
    public class UserRoleAssignedEventHandler : INotificationHandler<UserRoleAssignedEvent>
    {
        public Task Handle(UserRoleAssignedEvent notification, CancellationToken cancellationToken)
        {
            // 处理角色分配事件，比如发送通知、记录审计等
            return Task.CompletedTask;
        }
    }
}