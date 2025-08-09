using ECommerce.BuildingBlocks.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 使用通用网关服务配置
builder.Services.AddGatewayCommonServices(builder.Configuration);
builder.Services.AddGatewayAuthorization();

builder.Host.UseSerilog();

var app = builder.Build();

// 启动时打印服务名
Log.Information("----------启动 Gateway 服务----------");

// 使用通用网关中间件配置
app.UseGatewayCommonMiddleware(builder.Environment);

app.Run();
