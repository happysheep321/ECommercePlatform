using ECommerce.BuildingBlocks.Logging;
using ECommerce.BuildingBlocks.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using ECommerce.BuildingBlocks.Authentication.Attributes;

var builder = WebApplication.CreateBuilder(args);

// Read JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings!.SecretKey);

// Add Authentication Service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // 这里不要写死策略名，而是通配所有权限
    options.AddPolicy("*", policy =>
    {
        // 默认策略可以要求认证用户
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new PermissionRequirement("*"));
    });
});

// 注册我们的自定义授权处理器
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Add services to the container.
SerilogConfiguration.ConfigureSerilog(builder.Configuration, "Gateway");
builder.Host.UseSerilog();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Cross-Domain Policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// 启动时打印服务名
Log.Information("----------启动 Gateway 服务----------");

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy(proxyPipeline =>
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

app.Run();
