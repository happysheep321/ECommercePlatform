using MediatR;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class LoginUserCommand : IRequest<LoginResultDto>
    {
        public required string UserName { get; set; }

        public required string Password { get; set; }
    }
}
