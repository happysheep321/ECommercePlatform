# 📋 开发规范与最佳实践

## 🎯 编码规范

### 1. 命名约定

#### C# 命名规范
```csharp
// ✅ 正确示例
public class UserService                    // 类名：PascalCase
{
    private readonly IUserRepository _userRepository;  // 私有字段：_camelCase
    
    public async Task<User> GetUserAsync(Guid userId)  // 方法名：PascalCase
    {
        var currentUser = await _userRepository.GetByIdAsync(userId);  // 变量：camelCase
        return currentUser;
    }
    
    public const string DEFAULT_ROLE = "User";         // 常量：UPPER_SNAKE_CASE
}

// ❌ 错误示例
public class userservice                    // 类名应该PascalCase
{
    private readonly IUserRepository userRepository;   // 私有字段缺少下划线前缀
    
    public async Task<User> get_user_async(Guid userid)  // 方法名不应该snake_case
    {
        var CurrentUser = await userRepository.GetByIdAsync(userid);  // 变量不应该PascalCase
        return CurrentUser;
    }
}
```

#### 文件和文件夹命名
```
✅ 正确：
Controllers/UserController.cs
Services/UserService.cs  
Models/UserProfile.cs
DTOs/UserDto.cs

❌ 错误：
controllers/user_controller.cs
services/userservice.cs
models/userprofile.cs
```

### 2. 代码结构

#### 类的组织结构
```csharp
public class UserService : IUserService
{
    #region Private Fields
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    #endregion

    #region Constructor
    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    #endregion

    #region Public Methods
    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        // 实现逻辑
    }
    #endregion

    #region Private Methods
    private bool ValidateUser(User user)
    {
        // 验证逻辑
    }
    #endregion
}
```

### 3. 注释规范

#### XML文档注释
```csharp
/// <summary>
/// 用户服务，负责用户相关的业务逻辑处理
/// 
/// 主要功能：
/// - 用户注册和登录
/// - 用户资料管理
/// - 用户地址管理
/// </summary>
/// <remarks>
/// 该服务遵循DDD架构思想，通过聚合根维护数据一致性
/// </remarks>
public class UserService : IUserService
{
    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="request">用户创建请求，包含用户名、邮箱等信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建成功的用户对象</returns>
    /// <exception cref="DomainException">当用户名或邮箱已存在时抛出</exception>
    /// <example>
    /// <code>
    /// var request = new CreateUserRequest 
    /// { 
    ///     Username = "john_doe", 
    ///     Email = "john@example.com" 
    /// };
    /// var user = await userService.CreateUserAsync(request);
    /// </code>
    /// </example>
    public async Task<User> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: 实现用户创建逻辑
        // HACK: 临时解决方案，需要优化
        // FIXME: 修复用户名验证逻辑
        // NOTE: 这里需要注意并发安全问题
    }
}
```

## 🏗️ DDD架构规范

### 1. 领域层规范

#### 实体设计
```csharp
/// <summary>
/// 用户聚合根
/// 负责维护用户相关的业务规则和数据一致性
/// </summary>
public class User : Entity, IAggregateRoot
{
    private readonly List<UserAddress> _addresses = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    // 只通过构造函数和方法修改状态，属性只读
    public string Username { get; private set; }
    public string Email { get; private set; }
    
    // 业务方法体现领域概念
    public void AddAddress(UserAddress address)
    {
        // 业务规则验证
        if (_addresses.Count >= 10)
            throw new DomainException("用户最多只能添加10个地址");
            
        _addresses.Add(address);
        
        // 发布领域事件
        AddDomainEvent(new UserAddressAddedEvent(this.Id, address.Id));
    }
    
    // 领域事件管理
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void ClearDomainEvents() => _domainEvents.Clear();
    
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
```

#### 值对象设计
```csharp
/// <summary>
/// 地址值对象
/// 通过属性值判断相等性，不可变
/// </summary>
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string ZipCode { get; private set; }
    
    public Address(string street, string city, string zipCode)
    {
        Street = Guard.Against.NullOrEmpty(street, nameof(street));
        City = Guard.Against.NullOrEmpty(city, nameof(city));
        ZipCode = Guard.Against.NullOrEmpty(zipCode, nameof(zipCode));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return ZipCode;
    }
}
```

### 2. 应用层规范

#### 命令处理器
```csharp
/// <summary>
/// 用户注册命令处理器
/// 遵循单一职责原则，只处理用户注册业务
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    
    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. 业务规则验证
        await ValidateBusinessRules(request);
        
        // 2. 创建领域对象
        var user = CreateUser(request);
        
        // 3. 持久化
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        // 4. 返回结果
        return new RegisterUserResult { UserId = user.Id };
    }
    
    private async Task ValidateBusinessRules(RegisterUserCommand request)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new DomainException("用户名已存在");
    }
}
```

### 3. 基础设施层规范

#### 仓储实现
```csharp
/// <summary>
/// 用户仓储实现
/// 负责用户聚合的持久化操作
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context) { }
    
    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Username == username);
    }
    
    // 使用规约模式处理复杂查询
    public async Task<IEnumerable<User>> FindAsync(ISpecification<User> specification)
    {
        return await _context.Users
            .Where(specification.ToExpression())
            .ToListAsync();
    }
}
```

## 📊 数据库规范

### 1. 实体配置
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 表名
        builder.ToTable("Users");
        
        // 主键
        builder.HasKey(u => u.Id);
        
        // 属性配置
        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();
            
        // 索引
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        // 值对象配置
        builder.OwnsOne(u => u.Profile, p =>
        {
            p.Property(profile => profile.FirstName).HasMaxLength(50);
            p.Property(profile => profile.LastName).HasMaxLength(50);
        });
        
        // 一对多关系
        builder.HasMany(u => u.Addresses)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 2. 迁移命名
```bash
# ✅ 正确命名
dotnet ef migrations add AddUserAddressEntity
dotnet ef migrations add UpdateUserProfileFields
dotnet ef migrations add CreateOrderTables

# ❌ 错误命名  
dotnet ef migrations add Migration1
dotnet ef migrations add Update
dotnet ef migrations add Fix
```

## 🧪 测试规范

### 1. 单元测试
```csharp
[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IPasswordHasher> _mockPasswordHasher;
    private UserService _userService;
    
    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _userService = new UserService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }
    
    [TestMethod]
    public async Task CreateUserAsync_WhenValidInput_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var request = new CreateUserRequest 
        { 
            Username = "testuser", 
            Email = "test@example.com" 
        };
        
        _mockUserRepository.Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        var result = await _userService.CreateUserAsync(request);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("testuser", result.Username);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
    
    [TestMethod]
    public async Task CreateUserAsync_WhenUsernameExists_ShouldThrowDomainException()
    {
        // Arrange
        var request = new CreateUserRequest { Username = "existing" };
        _mockUserRepository.Setup(r => r.ExistsByUsernameAsync("existing"))
            .ReturnsAsync(true);
        
        // Act & Assert
        await Assert.ThrowsExceptionAsync<DomainException>(
            () => _userService.CreateUserAsync(request));
    }
}
```

### 2. 集成测试
```csharp
[TestClass]
public class UserControllerIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task RegisterUser_WhenValidInput_ShouldReturn201()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Username = "integrationtest",
            Email = "integration@test.com",
            Password = "Password123!"
        };
        
        // Act
        var response = await Client.PostAsync("/api/auth/register", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RegisterUserResponse>(content);
        Assert.IsNotNull(result.UserId);
    }
}
```

## 🔒 安全规范

### 1. 输入验证
```csharp
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .Length(3, 50).WithMessage("用户名长度必须在3-50字符之间")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码至少8位")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("密码必须包含大小写字母、数字和特殊字符");
    }
}
```

### 2. 敏感信息处理
```csharp
// ✅ 正确：不在日志中记录敏感信息
_logger.LogInformation("用户 {Username} 尝试登录", request.Username);

// ❌ 错误：不要记录密码等敏感信息
_logger.LogInformation("登录请求: {@Request}", request); // request包含密码

// ✅ 正确：密码哈希存储
user.SetPassword(_passwordHasher.HashPassword(password));

// ❌ 错误：明文存储密码
user.Password = password;
```

## 📈 性能规范

### 1. 数据库查询优化
```csharp
// ✅ 正确：使用Include预加载
var users = await _context.Users
    .Include(u => u.Profile)
    .Include(u => u.Addresses)
    .Where(u => u.IsActive)
    .ToListAsync();

// ❌ 错误：N+1查询问题
var users = await _context.Users.ToListAsync();
foreach (var user in users)
{
    var profile = await _context.UserProfiles.FirstAsync(p => p.UserId == user.Id);
}

// ✅ 正确：分页查询
var users = await _context.Users
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 2. 缓存策略
```csharp
public async Task<User> GetUserAsync(Guid userId)
{
    var cacheKey = $"user:{userId}";
    
    // 先从缓存获取
    var cachedUser = await _cache.GetAsync<User>(cacheKey);
    if (cachedUser != null)
        return cachedUser;
    
    // 缓存未命中，从数据库获取
    var user = await _userRepository.GetByIdAsync(userId);
    if (user != null)
    {
        await _cache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));
    }
    
    return user;
}
```

## 🔄 Git工作流规范

### 1. 分支命名
```bash
# 功能分支
feature/user-registration
feature/shopping-cart
feature/payment-integration

# 修复分支
bugfix/login-validation-error
hotfix/critical-security-patch

# 发布分支
release/v1.0.0
release/v1.1.0
```

### 2. 提交信息格式
```bash
# 格式：<type>(<scope>): <subject>

# 功能
feat(auth): 添加用户注册功能
feat(cart): 实现购物车商品添加

# 修复
fix(login): 修复密码验证逻辑错误
fix(api): 修复用户地址更新问题

# 文档
docs(readme): 更新API文档
docs(guide): 添加DDD架构说明

# 重构
refactor(user): 优化用户服务结构
refactor(db): 重构数据库配置

# 测试
test(user): 添加用户服务单元测试
test(integration): 完善集成测试用例

# 样式
style(format): 统一代码格式
style(naming): 规范变量命名

# 性能
perf(query): 优化用户查询性能
perf(cache): 改进缓存策略
```

### 3. 代码审查清单

#### 审查要点
- [ ] 代码是否遵循命名规范
- [ ] 是否有充分的单元测试
- [ ] 是否有适当的错误处理
- [ ] 是否有性能问题
- [ ] 是否有安全隐患
- [ ] 是否符合DDD架构原则
- [ ] 文档是否完整

## 🗄️ EF Core 迁移管理

### 1. 迁移命名规范
```bash
# ✅ 推荐的命名方式
dotnet ef migrations add AddUserProfileFields
dotnet ef migrations add UpdateProductPricing
dotnet ef migrations add CreateOrderTables

# ❌ 不推荐的命名方式
dotnet ef migrations add Migration1
dotnet ef migrations add Update
dotnet ef migrations add Fix
```

### 2. 迁移操作流程

#### 开发阶段
```bash
# 1. 修改实体模型
# 2. 检查当前迁移状态
dotnet ef migrations list

# 3. 创建迁移
dotnet ef migrations add DescriptiveMigrationName

# 4. 检查生成的迁移文件
# 5. 更新数据库
dotnet ef database update
```

#### 生产环境
```bash
# 1. 检查当前迁移状态
dotnet ef migrations list

# 2. 生成SQL脚本
dotnet ef migrations script --output production-migration.sql

# 3. 检查SQL脚本
# 4. 在数据库中执行脚本
```

#### 撤销操作
```bash
# 1. 检查当前迁移状态
dotnet ef migrations list

# 2. 移除最后一个迁移（需要数据库连接）
dotnet ef migrations remove
```

### 3. 使用PowerShell脚本

#### 推荐执行顺序
```powershell
# 1. 检查当前状态
.\scripts\ef-migrations.ps1 identity list

# 2. 创建新迁移
.\scripts\ef-migrations.ps1 identity add -MigrationName "AddUserProfileFields"

# 3. 生成SQL脚本（可选，用于生产环境）
.\scripts\ef-migrations.ps1 identity script -OutputPath "migration.sql"

# 4. 更新数据库
.\scripts\ef-migrations.ps1 identity update

# 5. 移除迁移（如果需要撤销）
.\scripts\ef-migrations.ps1 identity remove
```

#### 多服务迁移顺序
```powershell
# 建议按依赖关系顺序执行
# 1. Identity服务（基础服务）
.\scripts\ef-migrations.ps1 identity add -MigrationName "InitialIdentitySchema"

# 2. Product服务（商品服务）
.\scripts\ef-migrations.ps1 product add -MigrationName "InitialProductSchema"

# 3. Cart服务（依赖Product）
.\scripts\ef-migrations.ps1 cart add -MigrationName "InitialCartSchema"

# 4. Order服务（依赖Product和Cart）
.\scripts\ef-migrations.ps1 order add -MigrationName "InitialOrderSchema"
```

### 4. API接口管理
每个微服务实现自己的迁移控制器，通过API接口管理迁移：

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MigrationController : ControllerBase
{
    private readonly IMigrationService _migrationService;
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateMigration([FromBody] CreateMigrationRequest request)
    {
        var projectPath = Directory.GetCurrentDirectory();
        var result = await _migrationService.CreateMigrationAsync(request.MigrationName, projectPath);
        // 处理结果...
    }
}
```

---

**记住**：好的代码不仅能运行，更要易读、易维护、易扩展！
