using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    /// <summary>
    /// 创建迁移命令验证器
    /// </summary>
    public class CreateMigrationCommandValidator : AbstractValidator<CreateMigrationCommand>
    {
        public CreateMigrationCommandValidator()
        {
            RuleFor(x => x.MigrationName)
                .NotEmpty().WithMessage("迁移名称不能为空")
                .MaximumLength(100).WithMessage("迁移名称不能超过100个字符")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("迁移名称只能包含字母、数字和下划线");
        }
    }
}
