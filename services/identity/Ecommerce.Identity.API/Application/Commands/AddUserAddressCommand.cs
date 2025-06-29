using Ecommerce.Identity.API.Domain.ValueObjects;
using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Application.Commands
{
    public class AddUserAddressCommand
    {
        public Guid UserId { get; private set; }
        public string? ReceiverName { get; private set; }
        public string? Phone { get; private set; }
        public Region? Region { get; private set; }
        public string? Detail { get; private set; }
        public bool IsDefault { get; private set; }
    }
}
