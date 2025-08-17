# MediatR与FluentValidation集成指南

## 📖 概述

本文档详细介绍了在ECommerce平台中如何正确集成MediatR和FluentValidation，确保数据验证规则能够正确执行。我们使用程序集注册的方式，自动发现所有Handler和验证器，大大简化了配置和维护工作。

## 🚨 常见问题

### 问题描述
初学者经常遇到这样的问题：即使定义了FluentValidation规则，但验证似乎没有生效，无效数据仍然被写入数据库。

**示例场景：**
```csharp
// 验证器定义
public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.Gender).InclusiveBetween(0, 2); // 性别：0=男，1=女，2=未知
    }
}

// 但是传入Gender=-1时，数据仍然被写入数据库！
```

### 问题原因
这个问题的根本原因是：**Command类没有正确集成到MediatR管道中，或者验证器没有正确注册**。

## 🔧 解决方案

### 1. 程序集注册方式（推荐）

#### 优势
- **自动发现**：自动发现所有Handler和验证器
- **减少维护成本**：添加新组件时不需要修改注册代码
- **一致性**：MediatR和验证器使用相同的注册模式
- **符合开闭原则**：对扩展开放，对修改封闭

### 2. 正确的服务注册

#### 在ServiceExtensions中注册
```csharp
// ✅ 正确的服务注册方式
public static IServiceCollection AddIdentityModule(
    this IServiceCollection services,
    IConfiguration config,
    IWebHostEnvironment env)
{
    // ===================== 1. 通用微服务配置 =====================
    services.AddMicroserviceCommonServices(
        configuration: config,
        serviceName: "ECommerce.Identity.API",
        swaggerTitle: "ECommerce.Identity.API",
        enableJwtAuth: true,
        enableRedis: true,
        mediatRAssembly: Assembly.GetExecutingAssembly(),    // 自动发现所有Handler
        validatorAssembly: Assembly.GetExecutingAssembly()   // 自动发现所有验证器
    );

    // ===================== 2. Pipeline 行为 =====================
    // 注册ValidationBehavior以支持IRequest和IRequest<TResponse>
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    return services;
}
```

### 3. 统一的ValidationBehavior实现

#### 支持所有类型的请求
```csharp
// ✅ 统一的ValidationBehavior实现
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Identity.API.Infrastructure.Behaviors
{
    /// <summary>
    /// 统一的ValidationBehavior，支持处理所有类型的请求
    /// 这个实现可以处理：
    /// - IRequest&lt;TResponse&gt; 类型的请求（有返回值）
    /// - IRequest 类型的请求（无返回值，返回Unit）
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> logger;
        
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger) 
        {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await ValidateRequest(request, cancellationToken);
            return await next();
        }

        private async Task ValidateRequest(TRequest request, CancellationToken cancellationToken)
        {
            var requestType = typeof(TRequest).Name;
            logger.LogInformation($"开始验证请求: {requestType}");

            if (validators != null && validators.Any())
            {
                logger.LogInformation($"找到 {validators.Count()} 个验证器用于 {requestType}");
                
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
                    logger.LogWarning($"验证失败: {requestType} - {string.Join(", ", errorMessages)}");
                    throw new ValidationException($"请求验证失败: {string.Join(", ", errorMessages)}");
                }
                
                logger.LogInformation($"验证通过: {requestType}");
            }
            else
            {
                logger.LogWarning($"未找到 {requestType} 的验证器");
            }
        }
    }
}
            else
            {
                logger.LogWarning($"未找到 {requestType} 的验证器");
            }

            return await next();
        }
    }

    // 专门处理无返回值的请求
    public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest, Unit> 
        where TRequest : IRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<ValidationBehavior<TRequest>> logger;
        
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest>> logger) 
        {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await ValidateRequest(request, cancellationToken);
            return await next();
        }

        private async Task ValidateRequest(TRequest request, CancellationToken cancellationToken)
        {
            var requestType = typeof(TRequest).Name;
            logger.LogInformation($"开始验证请求: {requestType}");

            if (validators != null && validators.Any())
            {
                logger.LogInformation($"找到 {validators.Count()} 个验证器用于 {requestType}");
                
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
                    logger.LogWarning($"验证失败: {requestType} - {string.Join(", ", errorMessages)}");
                    throw new ValidationException($"请求验证失败: {string.Join(", ", errorMessages)}");
                }
                
                logger.LogInformation($"验证通过: {requestType}");
            }
            else
            {
                logger.LogWarning($"未找到 {requestType} 的验证器");
            }
        }
    }
}
```

### 4. 正确的实现方式

#### 步骤1：Command类实现正确的接口
```csharp
// ✅ 无返回值的Command
public class UpdateUserProfileCommand : IRequest
{
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string AvatarUrl { get; set; } = default!;
    public string NickName { get; set; } = default!;
    public int Gender { get; set; }
}

// ✅ 有返回值的Command
public class RegisterUserCommand : IRequest<Guid>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

#### 步骤2：创建对应的Handler
```csharp
// 无返回值的Handler
public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserService userService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UpdateUserProfileCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        this.userService = userService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await userService.UpdateProfileAsync(userId, request);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : throw new UnauthorizedAccessException();
    }
}

// 有返回值的Handler
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserService userService;

    public RegisterUserCommandHandler(IUserService userService)
    {
        this.userService = userService;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.RegisterAsync(request);
    }
}
```

#### 步骤3：Controller使用MediatR
```csharp
// ✅ 正确写法 - 通过MediatR调用，验证会执行
public class UserController : ControllerBase
{
    private readonly IMediator mediator;

    public UserController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateProfileAsync([FromBody] UpdateUserProfileCommand command)
    {
        await mediator.Send(command); // 这里会触发验证
        return Ok(ApiResponse<string>.Ok("OK", "更新成功"));
    }
}
```

## 📋 完整示例

### 1. 用户资料更新流程

```csharp
// 1. Command定义
public class UpdateUserProfileCommand : IRequest
{
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string AvatarUrl { get; set; } = default!;
    public string NickName { get; set; } = default!;
    public int Gender { get; set; }
}

// 2. 验证器定义
public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    private readonly ILogger<UpdateUserProfileCommandValidator> logger;

    public UpdateUserProfileCommandValidator(ILogger<UpdateUserProfileCommandValidator> logger)
    {
        this.logger = logger;
        
        logger.LogInformation("UpdateUserProfileCommandValidator 构造函数被调用");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("邮箱格式不正确");

        RuleFor(x => x.Phone)
            .Matches(@"^1[3-9]\d{9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("手机号格式不正确");

        RuleFor(x => x.AvatarUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl))
            .WithMessage("头像地址格式不合法");

        RuleFor(x => x.NickName)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.NickName))
            .WithMessage("昵称不能超过20个字符");

        RuleFor(x => x.Gender)
            .InclusiveBetween(0, 2)
            .WithMessage("性别只能是 0（男）、1（女）或 2（未知）");
    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}

// 3. Handler实现
public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserService userService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UpdateUserProfileCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        this.userService = userService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await userService.UpdateProfileAsync(userId, request);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : throw new UnauthorizedAccessException();
    }
}

// 4. Controller调用
[HttpPut("profile")]
public async Task<ActionResult<ApiResponse<string>>> UpdateProfileAsync([FromBody] UpdateUserProfileCommand command)
{
    await mediator.Send(command); // 这里会触发验证
    return Ok(ApiResponse<string>.Ok("OK", "更新成功"));
}
```

### 2. 用户注册流程（带返回值）

```csharp
// 1. Command定义
public class RegisterUserCommand : IRequest<Guid>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

// 2. 验证器定义
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码长度不能少于8位")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("密码必须包含大小写字母和数字");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("两次输入的密码不一致");
    }
}

// 3. Handler实现
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserService userService;

    public RegisterUserCommandHandler(IUserService userService)
    {
        this.userService = userService;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.RegisterAsync(request);
    }
}

// 4. Controller调用
[HttpPost("register")]
public async Task<ActionResult<ApiResponse<Guid>>> RegisterAsync([FromBody] RegisterUserCommand command)
{
    var userId = await mediator.Send(command); // 这里会触发验证并返回Guid
    return Ok(ApiResponse<Guid>.Ok(userId, "注册成功"));
}
```

## 🔍 调试技巧

### 1. 常见错误排查
- **问题：** 验证器没有执行
  - **检查：** 是否正确使用了程序集注册：`validatorAssembly: Assembly.GetExecutingAssembly()`
  - **检查：** Command类是否实现了正确的接口（`IRequest` 或 `IRequest<TResponse>`）
  - **检查：** Controller是否使用 `mediator.Send()` 而不是直接调用Service
  - **检查：** ValidationBehavior是否正确注册：`services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))`

- **问题：** 验证器执行了但数据仍然保存
  - **检查：** Handler中是否正确处理了验证异常
  - **检查：** Service层是否有额外的验证逻辑覆盖了FluentValidation

### 2. 验证验证器是否正常工作
```csharp
// 发送包含无效数据的请求来测试验证器
// 例如：发送Gender=-1的UpdateUserProfileCommand
{
  "email": "test@example.com",
  "phone": "13800138000",
  "avatarUrl": "https://example.com/avatar.jpg",
  "nickName": "测试用户",
  "gender": -1  // 这个值应该会触发验证错误
}

// 应该收到类似这样的错误响应：
{
  "success": false,
  "code": "VALIDATION_ERROR",
  "message": "请求验证失败: 性别只能是 0（男）、1（女）或 2（未知）"
}
```

## 📚 最佳实践

### 1. 程序集注册（推荐）
```csharp
// ✅ 推荐：使用程序集注册，自动发现所有组件
services.AddMicroserviceCommonServices(
    configuration: config,
    serviceName: "ECommerce.Identity.API",
    swaggerTitle: "ECommerce.Identity.API",
    enableJwtAuth: true,
    enableRedis: true,
    mediatRAssembly: Assembly.GetExecutingAssembly(),    // 自动发现所有Handler
    validatorAssembly: Assembly.GetExecutingAssembly()   // 自动发现所有验证器
);

// ❌ 不推荐：手动指定每个组件
mediatRAssemblies: new[]
{
    typeof(IUserService),
    typeof(UserRoleAssignedEvent),
    // ... 需要手动维护
},
validatorAssemblies: new[]
{
    typeof(RegisterUserCommandValidator),
    typeof(LoginUserCommandValidator),
    // ... 需要手动维护
}
```

### 2. 命名规范
```csharp
// Command命名：动词 + 名词 + Command
public class UpdateUserProfileCommand : IRequest
public class CreateOrderCommand : IRequest<Guid>
public class DeleteProductCommand : IRequest

// Handler命名：Command名 + Handler
public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>

// 验证器命名：Command名 + Validator
public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
```

### 3. 验证器组织
```csharp
// 将验证器放在Application.Validators命名空间下
namespace ECommerce.Identity.API.Application.Validators
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.Gender)
                .InclusiveBetween(0, 2)
                .WithMessage("性别只能是 0（男）、1（女）或 2（未知）");
            
            // 其他验证规则...
        }
    }
}
```

### 4. 错误处理
```csharp
// 在Controller中统一处理验证异常
[HttpPut("profile")]
public async Task<ActionResult<ApiResponse<string>>> UpdateProfileAsync([FromBody] UpdateUserProfileCommand command)
{
    try
    {
        await mediator.Send(command);
        return Ok(ApiResponse<string>.Ok("OK", "更新成功"));
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
        return BadRequest(ApiResponse<string>.Fail("VALIDATION_ERROR", string.Join("; ", errors)));
    }
}
```

### 5. 日志记录
```csharp
// 在ValidationBehavior中添加详细的日志记录
logger.LogInformation($"开始验证请求: {requestType}");
logger.LogInformation($"找到 {validators.Count()} 个验证器用于 {requestType}");
logger.LogInformation($"验证通过: {requestType}");
logger.LogWarning($"验证失败: {requestType} - {string.Join(", ", errorMessages)}");
```

## 🎯 总结

正确集成MediatR和FluentValidation的关键点：

### 1. **程序集注册（推荐）**
- 使用 `mediatRAssembly: Assembly.GetExecutingAssembly()` 自动发现所有Handler
- 使用 `validatorAssembly: Assembly.GetExecutingAssembly()` 自动发现所有验证器
- 减少维护成本，符合开闭原则

### 2. **统一的ValidationBehavior**
- 支持所有类型的请求：`IRequest` 和 `IRequest<TResponse>`
- 统一的验证逻辑，易于维护
- 详细的日志记录，便于调试

### 3. **正确的架构设计**
- Command类必须实现正确的接口：`IRequest` 或 `IRequest<TResponse>`
- 必须创建对应的Handler实现：`IRequestHandler` 或 `IRequestHandler<TRequest, TResponse>`
- Controller必须通过 `mediator.Send()` 调用
- 验证器会自动通过ValidationBehavior执行

### 4. **优势**
- **自动发现**：无需手动注册每个组件
- **一致性**：MediatR和验证器使用相同的注册模式
- **可维护性**：添加新组件时不需要修改注册代码
- **可扩展性**：其他微服务可以使用相同的模式

遵循这些原则，您的数据验证就会正确工作，确保数据质量和系统安全！

## 📚 相关文档

- [BuildingBlocks通用组件库](02_BuildingBlocks.md) - 了解基础设施组件
- [Identity身份认证服务](05_IdentityService.md) - 完整的身份认证服务实现
- [项目概述](01_项目概述.md) - 整体架构和技术栈介绍
