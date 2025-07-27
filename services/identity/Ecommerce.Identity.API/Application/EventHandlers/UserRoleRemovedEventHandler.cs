using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ecommerce.Identity.API.Domain.Events;

namespace Ecommerce.Identity.API.Application.EventHandlers
{
    public class UserRoleRemovedEventHandler : INotificationHandler<UserRoleRemovedEvent>
    {
        public Task Handle(UserRoleRemovedEvent notification, CancellationToken cancellationToken)
        {
            // 处理角色移除事件，比如发送通知、记录审计等
            return Task.CompletedTask;
        }
    }
}