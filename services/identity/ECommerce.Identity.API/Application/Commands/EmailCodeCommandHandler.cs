using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class EmailCodeCommandHandler : IRequestHandler<EmailCodeCommand>
    {
        private readonly IVerificationCodeService emailVerificationService;

        public EmailCodeCommandHandler(IVerificationCodeService emailVerificationService)
        {
            this.emailVerificationService = emailVerificationService;
        }

        public async Task Handle(EmailCodeCommand request, CancellationToken cancellationToken)
        {
            await emailVerificationService.SendCodeAsync(request.Email);
        }
    }
}
