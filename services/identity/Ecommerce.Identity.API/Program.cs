using Ecommerce.Identity.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// 一行加载 Identity 所有模块
builder.Services.AddIdentityModule(builder.Configuration, builder.Environment);

var app = builder.Build();

Log.Information("---------- 启动 Identity 微服务 ----------");

// Swagger 仅在开发环境开启
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// 网关访问限制中间件
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

app.MapControllers();
app.Run();
