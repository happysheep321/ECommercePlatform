using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class EmailCodeCommand : IRequest
    {
        public string Email { get; set; } = default!;
    }
}
