using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class UpdateUserAddressCommandValidator : AbstractValidator<UpdateUserAddressCommand>
    {
        public UpdateUserAddressCommandValidator()
        {
            RuleFor(x => x.AddressId)
                .NotEmpty().WithMessage("地址ID不能为空");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("用户ID不能为空");

            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("收件人姓名不能为空");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("手机号不能为空")
                .Matches(@"^1[3-9]\d{9}$").WithMessage("手机号格式不正确");

            RuleFor(x => x.Region)
                .NotNull().WithMessage("地区信息不能为空");

            RuleFor(x => x.Detail)
                .NotEmpty().WithMessage("详细地址不能为空");
        }
    }
}
