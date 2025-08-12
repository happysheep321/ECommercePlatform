using System.Threading.Tasks;

namespace ECommerce.BuildingBolcks.EFCore
{
    /// <summary>
    /// EF Core 迁移服务接口
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// 创建新的迁移
        /// </summary>
        /// <param name="migrationName">迁移名称</param>
        /// <param name="projectPath">项目路径</param>
        /// <param name="startupProjectPath">启动项目路径</param>
        /// <returns>操作结果</returns>
        Task<MigrationResult> CreateMigrationAsync(string migrationName, string projectPath, string? startupProjectPath = null);

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <param name="startupProjectPath">启动项目路径</param>
        /// <returns>操作结果</returns>
        Task<MigrationResult> UpdateDatabaseAsync(string projectPath, string? startupProjectPath = null);

        /// <summary>
        /// 移除最后一个迁移
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <param name="startupProjectPath">启动项目路径</param>
        /// <returns>操作结果</returns>
        Task<MigrationResult> RemoveLastMigrationAsync(string projectPath, string? startupProjectPath = null);

        /// <summary>
        /// 生成SQL脚本
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <param name="startupProjectPath">启动项目路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns>操作结果</returns>
        Task<MigrationResult> GenerateScriptAsync(string projectPath, string? startupProjectPath = null, string? outputPath = null);

        /// <summary>
        /// 列出所有迁移
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <param name="startupProjectPath">启动项目路径</param>
        /// <returns>迁移列表</returns>
        Task<MigrationListResult> ListMigrationsAsync(string projectPath, string? startupProjectPath = null);
    }

    /// <summary>
    /// 迁移操作结果
    /// </summary>
    public class MigrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Output { get; set; }
        public string? Error { get; set; }
    }

    /// <summary>
    /// 迁移列表结果
    /// </summary>
    public class MigrationListResult : MigrationResult
    {
        public List<string> Migrations { get; set; } = new List<string>();
    }
}
