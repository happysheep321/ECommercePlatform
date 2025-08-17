using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserService userService;

        public RegisterUserCommandHandler(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.RegisterAsync(request);
        }
    }
}
