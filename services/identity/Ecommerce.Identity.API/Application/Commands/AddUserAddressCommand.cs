using Ecommerce.Identity.API.Domain.ValueObjects;
using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Application.Commands
{
    public class AddUserAddressCommand
    {
        public string ReceiverName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public Region Region { get; set; } = default!;
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
    }
}
