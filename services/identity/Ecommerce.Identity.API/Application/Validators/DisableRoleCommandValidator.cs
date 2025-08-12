using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class DisableRoleCommandValidator : AbstractValidator<DisableRoleCommand>
    {
        public DisableRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}
