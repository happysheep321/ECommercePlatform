using MediatR;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
    {
        private readonly IUserService userService;

        public LoginUserCommandHandler(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.LoginAsync(request);
        }
    }
}
