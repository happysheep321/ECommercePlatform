using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserAddress : Entity<Guid>
    {
        public Guid UserId { get; private set; }

        public string ReceiverName { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string Region { get; private set; } = string.Empty;
        public string Detail { get; private set; } = string.Empty;

        public bool IsDefault { get; private set; }

        protected UserAddress()
        {
            
        }

        public UserAddress(Guid userId, string receiverName, string phone, string region, string detail, bool isDefault)
        {
            Id = Guid.NewGuid(); // 聚合根生成的ID
            UserId = userId;
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
            IsDefault = isDefault;
        }

        public void UpdateAddress(string receiverName, string phone, string region, string detail, bool isDefault)
        {
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
            IsDefault = isDefault;
        }

        public void SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }
}
