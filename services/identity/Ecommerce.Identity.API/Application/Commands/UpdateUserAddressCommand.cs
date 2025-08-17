using ECommerce.Identity.API.Domain.ValueObjects;

using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class UpdateUserAddressCommand : IRequest
{
    public Guid AddressId { get; set; }
    public string ReceiverName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public Region Region { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public bool IsDefault { get; set; }
}
}
