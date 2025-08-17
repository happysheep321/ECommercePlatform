using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IUserService userService;

        public ForgotPasswordCommandHandler(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            await userService.ResetPasswordByEmailAsync(request);
        }
    }
}
