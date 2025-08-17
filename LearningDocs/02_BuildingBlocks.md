# 02_BuildingBlocks - 通用组件库学习指南

## 【学习目标】
通过本文档，您将学会：
- 理解什么是通用组件库及其作用
- 掌握BuildingBlocks项目的结构和功能
- 学会使用各种扩展方法来配置微服务
- 理解依赖注入、中间件、日志等核心概念
- 能够独立配置一个基础的微服务项目

## 【什么是BuildingBlocks？】

### 概念解释
BuildingBlocks就像是一个**工具箱**，里面装满了所有微服务都会用到的工具：
- **日志记录工具**：记录系统运行信息
- **认证授权工具**：处理用户登录和权限
- **缓存工具**：提高系统性能
- **配置工具**：管理各种设置
- **中间件工具**：处理请求和响应

### 为什么需要BuildingBlocks？
想象一下，如果每个微服务都要自己写一遍日志、认证、缓存的代码：
- 代码重复，维护困难
- 标准不统一，容易出错
- 开发效率低

BuildingBlocks解决了这些问题：
- **代码复用**：写一次，所有服务都能用
- **标准统一**：所有服务使用相同的配置方式
- **快速开发**：几行代码就能配置好基础功能

## 【项目结构详解】

### 目录结构
```
ECommerce.BuildingBlocks/
├── Authentication/     # 认证授权相关
├── Configuration/      # 配置管理
├── EFCore/            # 数据库访问
├── EventBus/          # 事件总线
├── Extensions/        # 扩展方法
├── Logging/           # 日志记录
├── Middlewares/       # 中间件
└── Redis/             # 缓存服务
```

### 核心文件说明

#### 1. ServiceCollectionExtensions.cs
**作用：** 提供各种服务注册的扩展方法
**位置：** `Extensions/ServiceCollectionExtensions.cs`

这个文件就像是一个**配置手册**，告诉你怎么配置各种服务。

## 【核心功能详解】

### 1. JWT认证配置

#### 什么是JWT？
JWT（JSON Web Token）就像是一个**电子身份证**：
- 包含用户信息（用户名、角色等）
- 有有效期限制
- 可以验证真伪
- 不需要在服务器存储

#### 代码示例
```csharp
// 在Program.cs中使用
services.AddJwtAuthentication(configuration);

// 这个方法做了什么？
public static IServiceCollection AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // 1. 从配置文件读取JWT设置
    var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
    
    // 2. 配置认证选项
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // 3. 设置Token验证参数
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,        // 验证发行者
            ValidateAudience = true,      // 验证受众
            ValidateLifetime = true,      // 验证有效期
            ValidateIssuerSigningKey = true, // 验证签名密钥
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
    
    return services;
}
```

#### 配置文件示例
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here",
    "Issuer": "ECommercePlatform",
    "Audience": "ECommerceUsers",
    "ExpirationMinutes": 60
  }
}
```

### 2. Swagger文档配置

#### 什么是Swagger？
Swagger是一个**API文档生成工具**：
- 自动生成API文档
- 提供在线测试界面
- 显示请求参数和响应格式

#### 代码示例
```csharp
// 在Program.cs中使用
services.AddSwaggerDocumentation("我的API", "v1");

// 这个方法做了什么？
public static IServiceCollection AddSwaggerDocumentation(
    this IServiceCollection services,
    string title,
    string version = "v1",
    string? xmlDocumentPath = null)
{
    services.AddSwaggerGen(c =>
    {
        // 1. 设置文档信息
        c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
        
        // 2. 添加XML注释支持
        if (File.Exists(xmlDocumentPath))
        {
            c.IncludeXmlComments(xmlDocumentPath);
        }
        
        // 3. 配置JWT认证
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
    });
    
    return services;
}
```

### 3. Redis缓存配置

#### 什么是Redis？
Redis是一个**内存数据库**，主要用作缓存：
- 数据存储在内存中，访问速度快
- 支持多种数据结构（字符串、列表、哈希等）
- 可以设置过期时间

#### 代码示例
```csharp
// 在Program.cs中使用
services.AddRedisServices(configuration);

// 这个方法做了什么？
public static IServiceCollection AddRedisServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // 1. 获取Redis连接字符串
    var redisConnectionString = configuration.GetConnectionString("Redis");
    
    // 2. 注册Redis连接
    services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        return ConnectionMultiplexer.Connect(redisConnectionString);
    });
    
    // 3. 注册Redis帮助类
    services.AddScoped<IRedisHelper, RedisHelper>();
    
    return services;
}
```

#### 配置文件示例
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

### 4. CORS跨域配置

#### 什么是CORS？
CORS（跨域资源共享）解决浏览器安全限制：
- 允许网页从不同域名获取资源
- 保护用户免受恶意网站攻击

#### 代码示例
```csharp
// 在Program.cs中使用
services.AddCorsPolicy();

// 这个方法做了什么？
public static IServiceCollection AddCorsPolicy(
    this IServiceCollection services,
    string policyName = "AllowAll")
{
    services.AddCors(options =>
    {
        options.AddPolicy(policyName, policy =>
        {
            policy.AllowAnyOrigin()    // 允许任何来源
                  .AllowAnyHeader()    // 允许任何请求头
                  .AllowAnyMethod();   // 允许任何HTTP方法
        });
    });
    
    return services;
}
```

### 5. 日志配置

#### 什么是Serilog？
Serilog是一个**结构化日志库**：
- 支持多种输出目标（文件、控制台、数据库等）
- 结构化数据，便于分析
- 性能优异

#### 代码示例
```csharp
// 在Program.cs中使用
services.AddSerilogServices(configuration, "MyService");

// 这个方法做了什么？
public static IServiceCollection AddSerilogServices(
    this IServiceCollection services,
    IConfiguration configuration,
    string serviceName)
{
    // 1. 配置Serilog
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.WithProperty("ServiceName", serviceName)
        .CreateLogger();
    
    // 2. 注册日志服务
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSerilog(dispose: true);
    });
    
    return services;
}
```

## 【实际使用示例】

### 完整的Program.cs配置
```csharp
using ECommerce.BuildingBlocks.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. 添加基础Web服务
builder.Services.AddBaseWebServices();

// 2. 添加JWT认证
builder.Services.AddJwtAuthentication(builder.Configuration);

// 3. 添加Swagger文档
builder.Services.AddSwaggerDocumentation("用户服务API", "v1");

// 4. 添加Redis缓存
builder.Services.AddRedisServices(builder.Configuration);

// 5. 添加CORS策略
builder.Services.AddCorsPolicy();

// 6. 添加日志服务
builder.Services.AddSerilogServices(builder.Configuration, "UserService");

var app = builder.Build();

// 7. 配置中间件
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
```

## 【依赖注入详解】

### 什么是依赖注入？
依赖注入就像**点外卖**：
- 你不需要自己做饭（创建对象）
- 外卖员把饭送到你家（注入对象）
- 你只需要告诉外卖员你要什么（声明依赖）

### 三种生命周期
```csharp
// 1. Singleton（单例）- 整个应用只有一个实例
services.AddSingleton<IMyService, MyService>();

// 2. Scoped（作用域）- 每个HTTP请求一个实例
services.AddScoped<IMyService, MyService>();

// 3. Transient（瞬态）- 每次请求都创建新实例
services.AddTransient<IMyService, MyService>();
```

### 使用示例
```csharp
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    // 构造函数注入
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;  // 自动注入
        _logger = logger;           // 自动注入
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        _logger.LogInformation("获取用户列表");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}
```

## 【下一步】
接下来我们将学习 **SharedKernel（共享领域模型）**，这是所有微服务共享的基础数据模型。请查看 `03_SharedKernel.md` 文档。

---

**学习提示：**
- 这些扩展方法就像积木，可以组合使用
- 每个方法都有特定的作用，理解其原理很重要
- 配置文件是这些功能的基础，要确保配置正确
- 依赖注入是.NET Core的核心概念，要深入理解
