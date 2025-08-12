using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
            RuleFor(x => x.EmailVerifyCode).NotEmpty().Length(6);
        }
    }
}
