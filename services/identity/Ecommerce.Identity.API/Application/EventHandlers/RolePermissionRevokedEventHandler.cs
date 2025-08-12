using ECommerce.Identity.API.Domain.Events;
using MediatR;

namespace ECommerce.Identity.API.Application.EventHandlers
{
    public class RolePermissionRevokedEventHandler : INotificationHandler<RolePermissionRevokedEvent>
    {
        public Task Handle(RolePermissionRevokedEvent notification, CancellationToken cancellationToken)
        {
            // 处理角色分配事件，比如发送通知、记录审计等
            return Task.CompletedTask;
        }
    }
}
