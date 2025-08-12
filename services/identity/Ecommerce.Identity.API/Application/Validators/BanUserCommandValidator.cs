using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class BanUserCommandValidator : AbstractValidator<BanUserCommand>
    {
        public BanUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(128);
        }
    }
}
