using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    /// <summary>
    /// 生成脚本命令验证器
    /// </summary>
    public class GenerateScriptCommandValidator : AbstractValidator<GenerateScriptCommand>
    {
        public GenerateScriptCommandValidator()
        {
            When(x => !string.IsNullOrEmpty(x.OutputPath), () =>
            {
                RuleFor(x => x.OutputPath)
                    .Must(path => Path.GetExtension(path) == ".sql")
                    .WithMessage("输出文件必须是.sql文件");
            });
        }
    }
}
