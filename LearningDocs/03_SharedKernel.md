# 03_SharedKernel - 共享领域模型学习指南

## 【学习目标】
通过本文档，您将学会：
- 理解什么是共享领域模型及其重要性
- 掌握DDD中的核心概念：实体、值对象、聚合根
- 学会使用领域事件进行服务间通信
- 理解为什么需要共享内核
- 能够设计自己的领域模型

## 【什么是SharedKernel？】

### 概念解释
SharedKernel就像是一个**共享的字典**，所有微服务都使用相同的词汇：
- **实体（Entity）**：有唯一标识的对象，如用户、商品
- **值对象（Value Object）**：没有唯一标识的对象，如地址、价格
- **领域事件（Domain Event）**：业务发生时的事件通知
- **枚举（Enum）**：固定的选项值，如订单状态
- **常量（Constants）**：系统中固定的值

### 为什么需要SharedKernel？
想象一下，如果每个微服务都定义自己的用户模型：
- 用户服务：`User { Id, Name, Email }`
- 订单服务：`User { Id, Name, Phone }`
- 支付服务：`User { Id, Name, Address }`

问题：
- 数据不一致，容易出错
- 修改一个服务，其他服务也要改
- 代码重复，维护困难

SharedKernel解决了这些问题：
- **统一模型**：所有服务使用相同的定义
- **一致性**：确保数据格式一致
- **可维护性**：修改一处，所有地方都生效

## 【项目结构详解】

### 目录结构
```
ECommerce.SharedKernel/
├── Base/              # 基础类
│   ├── Entity.cs      # 实体基类
│   ├── EntityBase.cs  # 聚合根基类
│   └── ValueObject.cs # 值对象基类
├── Events/            # 领域事件
├── Interfaces/        # 接口定义
├── Enums/            # 枚举定义
├── DTOs/             # 数据传输对象
├── Constants/        # 常量定义
├── Exceptions/       # 异常定义
├── Authentication/   # 认证相关
├── Logging/          # 日志相关
└── Redis/            # 缓存相关
```

## 【核心概念详解】

### 1. 实体（Entity）

#### 什么是实体？
实体就像现实世界中的**具体事物**：
- 有唯一标识（ID）
- 有生命周期（创建、修改、删除）
- 可以改变状态
- 通过ID来识别

#### 代码示例
```csharp
// 实体基类
public abstract class Entity<TId>
{
    public TId? Id { get; protected set; }  // 唯一标识

    // 重写相等性比较
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        return Id!.Equals(other.Id);  // 通过ID比较
    }
    
    public override int GetHashCode()
    {
        return Id!.GetHashCode();  // 通过ID生成哈希码
    }
}

// 具体实体示例
public class User : Entity<Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    
    public User(string name, string email)
    {
        Id = Guid.NewGuid();  // 生成唯一ID
        Name = name;
        Email = email;
    }
    
    public void UpdateName(string newName)
    {
        Name = newName;  // 可以修改状态
    }
}
```

### 2. 聚合根（Aggregate Root）

#### 什么是聚合根？
聚合根就像是一个**家庭的主人**：
- 管理一组相关的实体
- 确保数据一致性
- 对外提供统一的访问接口
- 负责发布领域事件

#### 代码示例
```csharp
// 聚合根基类
public abstract class EntityBase<TId> : Entity<TId>, IAggregateRoot
{
    private List<IDomainEvent>? domainEvents;  // 领域事件列表
    
    // 只读的领域事件集合
    public IReadOnlyCollection<IDomainEvent>? DomainEvents => domainEvents?.AsReadOnly();

    // 添加领域事件
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents ??= new List<IDomainEvent>();
        domainEvents.Add(domainEvent);
    }

    // 移除领域事件
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents?.Remove(domainEvent);
    }

    // 清空领域事件
    public void ClearDomainEvents()
    {
        domainEvents?.Clear();
    }
}

// 具体聚合根示例
public class Order : EntityBase<Guid>
{
    private List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }

    public Order()
    {
        Id = Guid.NewGuid();
        Status = OrderStatus.Created;
    }

    // 添加订单项
    public void AddItem(Product product, int quantity)
    {
        var item = new OrderItem(product.Id, quantity, product.Price);
        _items.Add(item);
        
        // 计算总金额
        TotalAmount = _items.Sum(x => x.Subtotal);
        
        // 发布领域事件
        AddDomainEvent(new OrderItemAddedEvent(Id, product.Id, quantity));
    }

    // 确认订单
    public void Confirm()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("只能确认已创建的订单");
            
        Status = OrderStatus.Confirmed;
        
        // 发布领域事件
        AddDomainEvent(new OrderConfirmedEvent(Id, TotalAmount));
    }
}
```

### 3. 值对象（Value Object）

#### 什么是值对象？
值对象就像现实世界中的**概念**：
- 没有唯一标识
- 不可变（创建后不能修改）
- 通过属性值来比较
- 通常表示一个概念或度量

#### 代码示例
```csharp
// 值对象基类
public abstract class ValueObject
{
    // 获取用于比较的属性值
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
}

// 具体值对象示例
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string state, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
    }
}

public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    // 值对象的运算
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("不能添加不同货币");
            
        return new Money(Amount + other.Amount, Currency);
    }
}
```

### 4. 领域事件（Domain Event）

#### 什么是领域事件？
领域事件就像现实世界中的**通知**：
- 当业务发生时，发布事件
- 其他服务可以订阅这些事件
- 实现服务间的松耦合通信
- 支持异步处理

#### 代码示例
```csharp
// 领域事件接口
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }  // 事件发生时间
}

// 具体领域事件
public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }
    public decimal TotalAmount { get; }
    public DateTime OccurredOn { get; }

    public OrderCreatedEvent(Guid orderId, Guid userId, decimal totalAmount)
    {
        OrderId = orderId;
        UserId = userId;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}

public class OrderConfirmedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public DateTime OccurredOn { get; }

    public OrderConfirmedEvent(Guid orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}

// 领域事件分发器
public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents);
}

// 使用MediatR的事件分发器
public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatRDomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
```

### 5. 枚举（Enum）

#### 什么是枚举？
枚举就像是一个**固定的选项列表**：
- 预定义的常量值
- 类型安全
- 便于维护和扩展

#### 代码示例
```csharp
// 订单状态枚举
public enum OrderStatus
{
    Created = 1,      // 已创建
    Confirmed = 2,    // 已确认
    Paid = 3,         // 已支付
    Shipped = 4,      // 已发货
    Delivered = 5,    // 已送达
    Cancelled = 6     // 已取消
}

// 支付方式枚举
public enum PaymentMethod
{
    CreditCard = 1,   // 信用卡
    DebitCard = 2,    // 借记卡
    PayPal = 3,       // PayPal
    Alipay = 4,       // 支付宝
    WeChatPay = 5     // 微信支付
}

// 用户角色枚举
public enum UserRole
{
    Customer = 1,     // 客户
    Admin = 2,        // 管理员
    Moderator = 3     // 版主
}
```

## 【实际使用示例】

### 完整的订单聚合示例
```csharp
public class Order : EntityBase<Guid>
{
    private List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Address ShippingAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    public Order(Guid userId, Address shippingAddress)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Created;
        TotalAmount = new Money(0, "USD");
        CreatedAt = DateTime.UtcNow;
        
        // 发布订单创建事件
        AddDomainEvent(new OrderCreatedEvent(Id, UserId, TotalAmount.Amount));
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("只能向已创建的订单添加商品");

        var existingItem = _items.FirstOrDefault(x => x.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = new OrderItem(product.Id, quantity, product.Price);
            _items.Add(item);
        }

        // 重新计算总金额
        TotalAmount = new Money(_items.Sum(x => x.Subtotal.Amount), "USD");
        
        // 发布事件
        AddDomainEvent(new OrderItemAddedEvent(Id, product.Id, quantity));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("只能确认已创建的订单");

        if (!_items.Any())
            throw new InvalidOperationException("订单不能为空");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        
        // 发布事件
        AddDomainEvent(new OrderConfirmedEvent(Id, TotalAmount.Amount));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("已送达的订单不能取消");

        Status = OrderStatus.Cancelled;
        
        // 发布事件
        AddDomainEvent(new OrderCancelledEvent(Id));
    }
}

public class OrderItem : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Subtotal => new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    public OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("数量必须大于0");
            
        Quantity = newQuantity;
    }
}
```

## 【事件处理示例】

### 事件处理器
```csharp
// 订单创建事件处理器
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IInventoryService _inventoryService;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger,
        IInventoryService inventoryService)
    {
        _logger = logger;
        _inventoryService = inventoryService;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("订单 {OrderId} 已创建，用户 {UserId}", 
            notification.OrderId, notification.UserId);
            
        // 可以在这里处理库存检查、发送通知等
        await _inventoryService.ReserveInventoryAsync(notification.OrderId);
    }
}

// 订单确认事件处理器
public class OrderConfirmedEventHandler : INotificationHandler<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedEventHandler> _logger;
    private readonly IPaymentService _paymentService;

    public OrderConfirmedEventHandler(
        ILogger<OrderConfirmedEventHandler> logger,
        IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("订单 {OrderId} 已确认，金额 {Amount}", 
            notification.OrderId, notification.TotalAmount);
            
        // 可以在这里处理支付流程
        await _paymentService.CreatePaymentRequestAsync(notification.OrderId, notification.TotalAmount);
    }
}
```

## 【下一步】
接下来我们将学习 **API网关（APIGateway）**，这是整个微服务架构的统一入口。请查看 `04_APIGateway.md` 文档。

---

**学习提示：**
- 实体和值对象的区别很重要，要理解清楚
- 聚合根是DDD的核心概念，负责维护数据一致性
- 领域事件是实现服务间通信的重要手段
- 枚举和常量让代码更加清晰和易维护
