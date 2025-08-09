using ECommerce.BuildingBlocks.Extensions;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ECommerce.BuildingBlocks.Extensions
{
    /// <summary>
    /// 微服务模板 - 展示如何使用通用组件
    /// 符合DDD架构思想，提供标准化的微服务启动模式
    /// </summary>
    public static class MicroserviceTemplate
    {
        /// <summary>
        /// 基础微服务启动模板
        /// </summary>
        public static WebApplication CreateMicroservice(
            string serviceName,
            string swaggerTitle,
            bool enableJwtAuth = false,
            bool enableRedis = false,
            Type[]? mediatRAssemblies = null,
            Type[]? validatorAssemblies = null,
            string[]? args = null)
        {
            var builder = WebApplication.CreateBuilder(args ?? Environment.GetCommandLineArgs());

            // 使用通用微服务配置
            builder.Services.AddMicroserviceCommonServices(
                configuration: builder.Configuration,
                serviceName: serviceName,
                swaggerTitle: swaggerTitle,
                enableJwtAuth: enableJwtAuth,
                enableRedis: enableRedis,
                mediatRAssemblies: mediatRAssemblies,
                validatorAssemblies: validatorAssemblies
            );

            builder.Host.UseSerilog();

            var app = builder.Build();

            Log.Information($"----------启动 {serviceName} 微服务----------");

            // 使用通用微服务中间件配置
            app.UseMicroserviceCommonMiddleware(builder.Environment);

            return app;
        }

        /// <summary>
        /// 带网关访问控制的微服务启动模板
        /// </summary>
        public static WebApplication CreateMicroserviceWithGatewayControl(
            string serviceName,
            string swaggerTitle,
            bool enableJwtAuth = false,
            bool enableRedis = false,
            Type[]? mediatRAssemblies = null,
            Type[]? validatorAssemblies = null,
            string[]? args = null)
        {
            var app = CreateMicroservice(
                serviceName,
                swaggerTitle,
                enableJwtAuth,
                enableRedis,
                mediatRAssemblies,
                validatorAssemblies,
                args
            );

            // 添加网关访问控制
            app.UseGatewayAccessControl();

            return app;
        }
    }
}
