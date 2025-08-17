using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Identity.API.Application.Validators
{
    public class AvatarUploadValidator : AbstractValidator<IFormFile>
    {
        public AvatarUploadValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("请选择要上传的文件");

            RuleFor(x => x.Length)
                .GreaterThan(0)
                .WithMessage("文件不能为空")
                .LessThanOrEqualTo(2 * 1024 * 1024) // 2MB
                .WithMessage("文件大小不能超过2MB");

            RuleFor(x => x.ContentType)
                .Must(BeValidImageType)
                .WithMessage("只支持jpg、png、gif格式的图片");

            RuleFor(x => x.FileName)
                .Must(BeValidFileName)
                .WithMessage("文件名包含非法字符");
        }

        private bool BeValidImageType(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            var validTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            return validTypes.Contains(contentType.ToLowerInvariant());
        }

        private bool BeValidFileName(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // 检查文件名是否包含非法字符
            var invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(c => invalidChars.Contains(c));
        }
    }
}
