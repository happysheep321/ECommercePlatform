using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.BuildingBolcks.EFCore
{
    /// <summary>
    /// EF Core 迁移服务注册扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册 EF Core 迁移服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddEFCoreMigrationService(this IServiceCollection services)
        {
            services.AddScoped<IMigrationService, MigrationService>();
            return services;
        }
    }
}
