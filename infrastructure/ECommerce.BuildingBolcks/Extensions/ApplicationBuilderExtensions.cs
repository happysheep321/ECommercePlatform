using Microsoft.AspNetCore.Authorization;
using ECommerce.BuildingBlocks.Authentication.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using ECommerce.BuildingBlocks.Middlewares;

namespace ECommerce.BuildingBlocks.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 配置微服务通用中间件管道
        /// </summary>
        public static IApplicationBuilder UseMicroserviceCommonMiddleware(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            bool enableSwagger = true,
            bool enableAuthentication = true,
            bool enableAuthorization = true)
        {
            app.UseRouting();

            // 全局异常统一返回
            app.UseGlobalException();

            if (enableAuthentication)
            {
                app.UseAuthentication();
            }

            if (enableAuthorization)
            {
                app.UseAuthorization();
            }

            if (enableSwagger && env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }

        /// <summary>
        /// 配置网关通用中间件管道
        /// </summary>
        public static IApplicationBuilder UseGatewayCommonMiddleware(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy(proxyPipeline =>
                {
                    proxyPipeline.Use(async (context, next) =>
                    {
                        if (context.User.Identity?.IsAuthenticated == true)
                        {
                            var userId = context.User.FindFirst("sub")?.Value ?? "";
                            var role = context.User.FindFirst("role")?.Value ?? "";
                            context.Request.Headers["X-User-Id"] = userId;
                            context.Request.Headers["X-User-Role"] = role;
                        }
                        await next();
                    });
                });
            });

            return app;
        }

        /// <summary>
        /// 配置网关授权策略
        /// </summary>
        public static IServiceCollection AddGatewayAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("*", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new PermissionRequirement("*"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }

        /// <summary>
        /// 添加网关访问限制中间件
        /// </summary>
        public static IApplicationBuilder UseGatewayAccessControl(
            this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.TryGetValue("X-Gateway-Auth", out var header) || header != "true")
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden: Must access via gateway.");
                    return;
                }
                await next();
            });

            return app;
        }
    }
}
