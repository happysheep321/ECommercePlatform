using Ecommerce.Identity.API.Domain.ValueObjects;

namespace Ecommerce.Identity.API.Application.Commands
{
    public class UpdateUserAddressCommand
    {
        public Guid AddressId { get; set; }
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public Region Region { get; set; } = default!;
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
    }
}
