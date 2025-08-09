using Ecommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace Ecommerce.Identity.API.Application.Validators
{
    public class DisableRoleCommandValidator : AbstractValidator<DisableRoleCommand>
    {
        public DisableRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}
