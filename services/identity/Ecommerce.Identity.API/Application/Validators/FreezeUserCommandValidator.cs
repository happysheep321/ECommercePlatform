using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class FreezeUserCommandValidator : AbstractValidator<FreezeUserCommand>
    {
        public FreezeUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(128);
        }
    }
}
