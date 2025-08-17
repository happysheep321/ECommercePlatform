using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 生成EF Core迁移SQL脚本命令
    /// </summary>
    public class GenerateScriptCommand : IRequest
    {
        /// <summary>
        /// 输出文件路径（可选）
        /// </summary>
        public string? OutputPath { get; set; }
    }
}
