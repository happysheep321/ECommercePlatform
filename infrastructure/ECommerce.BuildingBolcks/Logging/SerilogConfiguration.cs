using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace ECommerce.BuildingBlocks.Logging;

/// <summary>
/// 提供 Serilog 日志配置的静态类
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// 配置并初始化 Serilog 日志记录器
    /// </summary>
    /// <param name="configuration">应用程序的配置对象</param>
    public static void ConfigureSerilog(IConfiguration configuration)
    {
        // 创建并配置 Serilog 日志记录器
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information() // 设置默认最小日志级别为 Information
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Microsoft 命名空间日志级别为 Warning
            .MinimumLevel.Override("System", LogEventLevel.Warning) // System 命名空间日志级别为 Warning
            .Enrich.FromLogContext() // 启用日志上下文信息
            .WriteTo.Console() // 输出到控制台
            .ReadFrom.Configuration(configuration) // 从配置文件读取其他设置
            .CreateLogger();

        // 设置全局日志记录器
        Log.Logger = logger;
    }
}
