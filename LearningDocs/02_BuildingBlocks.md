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

### 1. 通用服务注册模板

#### 什么是服务注册模板？
服务注册模板就像是一个**标准化的配置模板**，将常用的服务注册逻辑封装成可复用的方法。这样每个微服务都可以使用相同的配置方式，确保一致性和可维护性。

#### 核心模板方法

**1. 微服务通用配置模板**
```csharp
// 一行代码配置所有微服务基础功能
public static IServiceCollection AddMicroserviceCommonServices(
    this IServiceCollection services,
    IConfiguration configuration,
    string serviceName,
    string swaggerTitle,
    bool enableJwtAuth = false,
    bool enableRedis = false,
    Assembly? mediatRAssembly = null,
    Assembly? validatorAssembly = null)
{
    // 1. 基础Web服务
    services.AddBaseWebServices();
    
    // 2. Serilog日志配置
    services.AddSerilogServices(configuration, serviceName);
    
    // 3. Swagger文档配置
    var xmlDocumentPath = Path.Combine(AppContext.BaseDirectory, $"{serviceName}.xml");
    services.AddSwaggerDocumentation(swaggerTitle, xmlDocumentPath: xmlDocumentPath);
    
    // 4. JWT认证（可选）
    if (enableJwtAuth)
    {
        services.AddJwtAuthentication(configuration);
    }
    
    // 5. Redis缓存（可选）
    if (enableRedis)
    {
        services.AddRedisServices(configuration);
    }
    
    // 6. MediatR注册（可选）
    if (mediatRAssembly != null)
    {
        services.AddMediatRServices(mediatRAssembly);
    }
    
    // 7. FluentValidation注册（可选）
    if (validatorAssembly != null)
    {
        services.AddValidatorsFromAssembly(validatorAssembly);
    }
    
    return services;
}
```

**2. 网关服务配置模板**
```csharp
// 网关专用配置模板
public static IServiceCollection AddGatewayCommonServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddJwtAuthentication(configuration);
    services.AddCorsPolicy();
    services.AddSerilogServices(configuration, "Gateway");
    services.AddReverseProxy()
           .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    
    return services;
}
```

**3. 基础Web服务模板**
```csharp
// 所有Web服务的基础配置
public static IServiceCollection AddBaseWebServices(
    this IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddHttpContextAccessor();
    
    return services;
}
```

#### 使用示例

**在微服务中使用：**
```csharp
// Program.cs - 一行配置所有基础功能
var builder = WebApplication.CreateBuilder(args);

// 使用通用模板配置微服务
builder.Services.AddMicroserviceCommonServices(
    configuration: builder.Configuration,
    serviceName: "ECommerce.Identity.API",
    swaggerTitle: "ECommerce.Identity.API",
    enableJwtAuth: true,           // 启用JWT认证
    enableRedis: true,             // 启用Redis缓存
    mediatRAssembly: Assembly.GetExecutingAssembly(),    // 自动发现Handler
    validatorAssembly: Assembly.GetExecutingAssembly()   // 自动发现验证器
);

var app = builder.Build();
```

**在网关中使用：**
```csharp
// Program.cs - 网关配置
var builder = WebApplication.CreateBuilder(args);

// 使用网关专用模板
builder.Services.AddGatewayCommonServices(builder.Configuration);

var app = builder.Build();
```

#### 模板的优势

**1. 标准化配置**
- 所有微服务使用相同的配置方式
- 减少配置错误和不一致
- 便于维护和升级

**2. 快速开发**
- 一行代码配置所有基础功能
- 新服务可以快速搭建
- 减少重复代码

**3. 灵活配置**
- 通过参数控制功能开关
- 支持不同服务的个性化需求
- 易于扩展和定制

**4. 程序集自动发现**
- 自动发现Handler和验证器
- 无需手动注册每个组件
- 符合开闭原则

### 2. JWT认证配置

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

#### 传统方式（不推荐）
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

#### 模板方式（推荐）
```csharp
using ECommerce.BuildingBlocks.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 一行代码配置所有基础功能
builder.Services.AddMicroserviceCommonServices(
    configuration: builder.Configuration,
    serviceName: "ECommerce.Identity.API",
    swaggerTitle: "ECommerce.Identity.API",
    enableJwtAuth: true,
    enableRedis: true,
    mediatRAssembly: Assembly.GetExecutingAssembly(),
    validatorAssembly: Assembly.GetExecutingAssembly()
);

var app = builder.Build();

// 使用通用中间件配置
app.UseMicroserviceCommonMiddleware(builder.Environment);

app.Run();
```

### 模板模式的优势对比

| 方面 | 传统方式 | 模板方式 |
|------|----------|----------|
| **代码行数** | 20+ 行 | 1 行 |
| **配置一致性** | 容易出错 | 标准化 |
| **维护成本** | 高 | 低 |
| **新服务搭建** | 慢 | 快 |
| **功能扩展** | 需要修改多处 | 只需修改模板 |

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

## 【模板模式最佳实践】

### 1. 模板设计原则

#### 单一职责原则
每个模板方法只负责一个特定的配置领域：
```csharp
// ✅ 好的设计：职责单一
services.AddBaseWebServices();           // 只负责基础Web服务
services.AddJwtAuthentication(config);   // 只负责JWT认证
services.AddRedisServices(config);       // 只负责Redis缓存

// ❌ 不好的设计：职责混乱
services.AddEverything(config);          // 什么都做，难以维护
```

#### 开闭原则
模板应该对扩展开放，对修改封闭：
```csharp
// ✅ 好的设计：支持扩展
public static IServiceCollection AddMicroserviceCommonServices(
    this IServiceCollection services,
    IConfiguration configuration,
    string serviceName,
    string swaggerTitle,
    bool enableJwtAuth = false,          // 可选功能
    bool enableRedis = false,            // 可选功能
    Assembly? mediatRAssembly = null,    // 可选功能
    Assembly? validatorAssembly = null)  // 可选功能
{
    // 基础配置
    services.AddBaseWebServices();
    services.AddSerilogServices(configuration, serviceName);
    
    // 条件配置
    if (enableJwtAuth) services.AddJwtAuthentication(configuration);
    if (enableRedis) services.AddRedisServices(configuration);
    if (mediatRAssembly != null) services.AddMediatRServices(mediatRAssembly);
    if (validatorAssembly != null) services.AddValidatorsFromAssembly(validatorAssembly);
    
    return services;
}
```

### 2. 模板使用技巧

#### 参数化配置
使用配置文件控制模板行为：
```csharp
// appsettings.json
{
  "ServiceConfiguration": {
    "EnableJwtAuth": true,
    "EnableRedis": true,
    "EnableSwagger": true
  }
}

// Program.cs
var serviceConfig = configuration.GetSection("ServiceConfiguration").Get<ServiceConfiguration>();

services.AddMicroserviceCommonServices(
    configuration: configuration,
    serviceName: "MyService",
    swaggerTitle: "MyService API",
    enableJwtAuth: serviceConfig.EnableJwtAuth,
    enableRedis: serviceConfig.EnableRedis
);
```

#### 环境特定配置
根据环境使用不同的模板：
```csharp
var environment = builder.Environment.EnvironmentName;

if (environment == "Development")
{
    services.AddMicroserviceCommonServices(
        configuration: configuration,
        serviceName: serviceName,
        swaggerTitle: swaggerTitle,
        enableJwtAuth: true,
        enableRedis: false  // 开发环境可能不需要Redis
    );
}
else if (environment == "Production")
{
    services.AddMicroserviceCommonServices(
        configuration: configuration,
        serviceName: serviceName,
        swaggerTitle: swaggerTitle,
        enableJwtAuth: true,
        enableRedis: true   // 生产环境需要Redis
    );
}
```

### 3. 模板扩展方法

#### 创建业务特定模板
```csharp
// 为电商平台创建专用模板
public static IServiceCollection AddECommerceServices(
    this IServiceCollection services,
    IConfiguration configuration,
    string serviceName)
{
    // 基础微服务配置
    services.AddMicroserviceCommonServices(
        configuration: configuration,
        serviceName: serviceName,
        swaggerTitle: serviceName,
        enableJwtAuth: true,
        enableRedis: true
    );
    
    // 电商平台特有配置
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
    services.AddScoped<IPermissionService, PermissionService>();
    
    return services;
}
```

#### 创建中间件模板
```csharp
// 中间件配置模板
public static IApplicationBuilder UseMicroserviceCommonMiddleware(
    this IApplicationBuilder app,
    IWebHostEnvironment environment)
{
    // 基础中间件
    app.UseRouting();
    app.UseCors("AllowAll");
    
    // 认证授权
    app.UseAuthentication();
    app.UseAuthorization();
    
    // 开发环境特有
    if (environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    // 端点配置
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
    
    return app;
}
```

### 4. 模板测试策略

#### 单元测试模板方法
```csharp
[Test]
public void AddMicroserviceCommonServices_WithValidParameters_ShouldRegisterAllServices()
{
    // Arrange
    var services = new ServiceCollection();
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "test-secret-key"},
            {"ConnectionStrings:Redis", "localhost:6379"}
        })
        .Build();
    
    // Act
    services.AddMicroserviceCommonServices(
        configuration: configuration,
        serviceName: "TestService",
        swaggerTitle: "Test API",
        enableJwtAuth: true,
        enableRedis: true
    );
    
    // Assert
    var serviceProvider = services.BuildServiceProvider();
    
    // 验证基础服务是否注册
    Assert.IsNotNull(serviceProvider.GetService<IControllerFactory>());
    Assert.IsNotNull(serviceProvider.GetService<IHttpContextAccessor>());
    
    // 验证JWT服务是否注册
    Assert.IsNotNull(serviceProvider.GetService<IAuthenticationService>());
    
    // 验证Redis服务是否注册
    Assert.IsNotNull(serviceProvider.GetService<IRedisHelper>());
}
```

## 【下一步】
接下来我们将学习 **SharedKernel（共享领域模型）**，这是所有微服务共享的基础数据模型。请查看 `03_SharedKernel.md` 文档。

---

**学习提示：**
- 服务注册模板是BuildingBlocks的核心功能，掌握它能让开发效率大幅提升
- 模板设计要遵循SOLID原则，确保可维护性和可扩展性
- 合理使用参数化配置，让模板更加灵活
- 为特定业务场景创建专用模板，提高代码复用性
- 依赖注入是.NET Core的核心概念，要深入理解
