using ECommerce.Identity.API.Extensions;
using ECommerce.BuildingBlocks.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 一行加载 Identity 所有模块
builder.Services.AddIdentityModule(builder.Configuration, builder.Environment);

builder.Host.UseSerilog();

var app = builder.Build();

Log.Information("---------- 启动 Identity 微服务 ----------");

// 使用通用微服务中间件配置
app.UseMicroserviceCommonMiddleware(builder.Environment);

// 网关访问限制中间件
app.UseGatewayAccessControl();

app.Run();
