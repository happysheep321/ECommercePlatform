using Ecommerce.Identity.API.Domain.ValueObjects;
using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserAddress : Entity<Guid>
    {
        public Guid UserId { get; private set; }

        public User User { get; private set; } = default!;

        public string ReceiverName { get; private set; } = string.Empty;

        public string Phone { get; private set; } = string.Empty;

        public Region? Region { get; private set; }

        public string Detail { get; private set; } = string.Empty;

        public bool IsDefault { get; private set; }

        protected UserAddress()
        {
            
        }

        public UserAddress(Guid id,Guid userId, string receiverName, string phone, Region region, string detail, bool isDefault)
        {
            Id = id;
            UserId = userId;
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
            IsDefault = isDefault;
        }

        public void UpdateAddress(string receiverName, string phone, Region region, string detail)
        {
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
        }

        public void SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }
}
