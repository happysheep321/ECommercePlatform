using ECommerce.BuildingBlocks.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 使用通用微服务配置
builder.Services.AddMicroserviceCommonServices(
    configuration: builder.Configuration,
    serviceName: "Cart",
    swaggerTitle: "ECommerce.Cart.API",
    enableJwtAuth: false,
    enableRedis: false
);

builder.Host.UseSerilog();

var app = builder.Build();

Log.Information("----------启动 Cart 微服务----------");

// 使用通用微服务中间件配置
app.UseMicroserviceCommonMiddleware(builder.Environment);

app.Run();
