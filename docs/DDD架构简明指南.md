# 🏗️ DDD架构简明指南

## 📖 什么是DDD？

**领域驱动设计（Domain-Driven Design, DDD）** 是一种软件开发方法论，强调围绕业务领域来构建软件系统。

### 🎯 核心理念

1. **业务优先**：代码结构反映业务结构
2. **通用语言**：开发团队与业务专家使用相同的术语
3. **边界清晰**：明确定义不同业务领域的边界

## 🏛️ 项目中的DDD实现

### 1. **分层架构**

```
┌─────────────────────────────────────┐
│          Presentation Layer         │  ← Controllers (API接口层)
├─────────────────────────────────────┤
│          Application Layer          │  ← Commands/Queries (应用层)
├─────────────────────────────────────┤
│            Domain Layer             │  ← Entities/ValueObjects (领域层)
├─────────────────────────────────────┤
│         Infrastructure Layer        │  ← Repositories/DbContext (基础设施层)
└─────────────────────────────────────┘
```

### 2. **领域模型示例**

#### 🧑 用户聚合根 (User Aggregate)
```csharp
public class User : Entity, IAggregateRoot
{
    // 聚合根负责维护一致性
    public void AddAddress(UserAddress address)
    {
        if (_addresses.Count >= 10)
            throw new DomainException("用户最多只能添加10个地址");
            
        _addresses.Add(address);
        // 发布领域事件
        AddDomainEvent(new UserAddressAddedEvent(this.Id, address));
    }
}
```

#### 📦 值对象 (Value Object)
```csharp
public class UserProfile : ValueObject
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    // 值对象通过属性值来判断相等性
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
```

### 3. **CQRS模式**

#### 命令 (Commands) - 修改数据
```csharp
public class RegisterUserCommand : IRequest<RegisterUserResult>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```

#### 查询 (Queries) - 读取数据
```csharp
public class GetUserProfileQuery : IRequest<UserProfileDto>
{
    public Guid UserId { get; set; }
}
```

### 4. **领域事件**

```csharp
// 事件定义
public class UserRegisteredEvent : IDomainEvent
{
    public Guid UserId { get; }
    public DateTime OccurredOn { get; }
}

// 事件处理
public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // 发送欢迎邮件等业务逻辑
    }
}
```

## 🎨 实际应用

### 在本项目中的体现：

1. **Identity微服务** = 用户管理领域
2. **Product微服务** = 商品管理领域  
3. **Order微服务** = 订单管理领域
4. **Cart微服务** = 购物车领域

每个微服务都是一个**限界上下文（Bounded Context）**，有自己的领域模型。

## 📚 学习建议

### 1. **理解业务**
- 先理解电商业务流程
- 识别关键的业务概念和规则
- 与业务专家建立通用语言

### 2. **从简单开始**
- 先实现最基本的功能
- 逐步重构和完善
- 不要过早优化

### 3. **实践原则**
- **单一职责**：每个类只负责一个业务概念
- **封装变化**：将可能变化的逻辑封装起来
- **依赖倒置**：高层模块不依赖低层模块

## 🔧 与项目的对应关系（只保留映射，不讲用法）

- MediatR → 实现命令/查询与事件分发（用法见《通用组件使用指南》）
- FluentValidation → 输入验证（用法见《通用组件使用指南》）
- EF Core → 持久化与仓储实现（细节见代码）
- 领域事件 → 见各领域的 Events 与 Handler

## 📖 推荐阅读

1. **《领域驱动设计》** - Eric Evans (DDD圣经)
2. **《实现领域驱动设计》** - Vaughn Vernon (实践指南)
3. **《.NET微服务架构》** - Microsoft官方文档

---

**记住**：DDD不是银弹，它是一种思维方式。重要的是理解业务，然后用代码准确表达业务概念！
