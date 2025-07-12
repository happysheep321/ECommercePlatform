using Ecommerce.Identity.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureIdentityServices();

var app = builder.Build();

// Print service name on startup
Log.Information("----------启动 Identity 微服务----------");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Middleware to block direct access; requests must come through the gateway
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
