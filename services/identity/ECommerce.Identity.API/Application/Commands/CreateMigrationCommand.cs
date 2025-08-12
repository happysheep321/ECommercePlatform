namespace ECommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 创建EF Core迁移命令
    /// </summary>
    public class CreateMigrationCommand
    {
        /// <summary>
        /// 迁移名称
        /// </summary>
        public string MigrationName { get; set; } = string.Empty;
    }
}
