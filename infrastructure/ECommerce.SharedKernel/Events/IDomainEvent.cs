using MediatR;

namespace Ecommerce.SharedKernel.Events
{
    /// <summary>
    /// 领域事件接口，继承自MediatR的INotification接口
    /// </summary>
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}