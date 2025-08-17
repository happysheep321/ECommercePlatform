using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class RemoveAddressCommand : IRequest
{
    public Guid AddressId { get; set; }
}
}
