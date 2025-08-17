using ECommerce.Identity.API.Domain.ValueObjects;
using ECommerce.SharedKernel.Base;

using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class AddUserAddressCommand : IRequest
    {
        public string ReceiverName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public Region Region { get; set; } = default!;
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
    }
}
