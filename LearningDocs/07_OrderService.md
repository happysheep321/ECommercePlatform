# 07_OrderService - 订单管理服务学习指南

## 【学习目标】
通过本文档，您将学会：
- 理解订单管理服务的作用和重要性
- 掌握订单创建、支付、发货的完整流程
- 学会订单状态管理和状态机设计
- 理解订单与商品、用户的关联关系
- 掌握订单查询和统计功能
- 能够独立开发和维护订单管理功能

## 【什么是订单管理服务？】

### 概念解释
订单管理服务就像是一个**订单处理中心**：
- **订单创建**：用户下单时创建订单
- **订单支付**：处理订单支付流程
- **订单发货**：管理订单发货和物流
- **订单状态管理**：跟踪订单的完整生命周期
- **订单查询**：提供订单查询和统计功能
- **订单取消**：处理订单取消和退款

### 为什么需要订单管理服务？
在电商平台中：
- 需要统一管理所有订单信息
- 需要跟踪订单的完整生命周期
- 需要处理支付和发货流程
- 需要提供订单查询和统计功能
- 需要处理订单取消和退款

订单管理服务解决了这些问题：
- **统一管理**：所有订单信息集中存储和管理
- **状态跟踪**：实时跟踪订单状态变化
- **流程管理**：标准化支付和发货流程
- **数据统计**：提供订单数据统计和分析
- **异常处理**：处理订单取消、退款等异常情况

## 【核心功能详解】

### 1. 订单实体设计

#### 订单主实体
```csharp
public class Order : EntityBase<Guid>
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public OrderType Type { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public string? CouponCode { get; private set; }
    public decimal? CouponDiscount { get; private set; }
    public string? Remark { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    
    // 导航属性
    public virtual User User { get; private set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public virtual OrderAddress? ShippingAddress { get; private set; }
    public virtual OrderPayment? Payment { get; private set; }
    public virtual OrderShipping? Shipping { get; private set; }
}
```

#### 订单状态枚举
```csharp
public enum OrderStatus
{
    Pending = 0,        // 待支付
    Paid = 1,           // 已支付
    Processing = 2,     // 处理中
    Shipped = 3,        // 已发货
    Delivered = 4,      // 已送达
    Completed = 5,      // 已完成
    Cancelled = 6,      // 已取消
    Refunded = 7        // 已退款
}

public enum OrderType
{
    Normal = 0,         // 普通订单
    PreOrder = 1,       // 预售订单
    GroupBuy = 2,       // 团购订单
    FlashSale = 3       // 秒杀订单
}
```

#### 订单项实体
```csharp
public class OrderItem : EntityBase<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductSku { get; private set; } = string.Empty;
    public string? ProductImage { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public decimal FinalPrice { get; private set; }
    
    // 导航属性
    public virtual Order Order { get; private set; } = null!;
    public virtual Product Product { get; private set; } = null!;
}
```

### 2. 订单地址管理

#### 订单地址实体
```csharp
public class OrderAddress : EntityBase<Guid>
{
    public Guid OrderId { get; private set; }
    public string ReceiverName { get; private set; } = string.Empty;
    public string ReceiverPhone { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string DetailAddress { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    
    // 导航属性
    public virtual Order Order { get; private set; } = null!;
}
```

### 3. 订单支付管理

#### 订单支付实体
```csharp
public class OrderPayment : EntityBase<Guid>
{
    public Guid OrderId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public string? TransactionId { get; private set; }
    public string? PaymentUrl { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? FailureReason { get; private set; }
    
    // 导航属性
    public virtual Order Order { get; private set; } = null!;
}

public enum PaymentMethod
{
    Alipay = 0,         // 支付宝
    WeChatPay = 1,      // 微信支付
    BankCard = 2,       // 银行卡
    CashOnDelivery = 3  // 货到付款
}

public enum PaymentStatus
{
    Pending = 0,        // 待支付
    Processing = 1,     // 支付中
    Success = 2,        // 支付成功
    Failed = 3,         // 支付失败
    Refunded = 4        // 已退款
}
```

### 4. 订单物流管理

#### 订单物流实体
```csharp
public class OrderShipping : EntityBase<Guid>
{
    public Guid OrderId { get; private set; }
    public string TrackingNumber { get; private set; } = string.Empty;
    public string Carrier { get; private set; } = string.Empty;
    public ShippingStatus Status { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? ShippingNote { get; private set; }
    
    // 导航属性
    public virtual Order Order { get; private set; } = null!;
    public virtual ICollection<ShippingTracking> TrackingHistory { get; private set; } = new List<ShippingTracking>();
}

public enum ShippingStatus
{
    Pending = 0,        // 待发货
    Shipped = 1,        // 已发货
    InTransit = 2,      // 运输中
    Delivered = 3,      // 已送达
    Failed = 4          // 配送失败
}

public class ShippingTracking : EntityBase<Guid>
{
    public Guid OrderShippingId { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    
    // 导航属性
    public virtual OrderShipping OrderShipping { get; private set; } = null!;
}
```

## 【API接口设计】

### 订单管理接口

#### 订单CRUD操作
```csharp
// 获取订单列表
GET /api/order
Query Parameters:
- page: int (页码)
- pageSize: int (每页数量)
- status: OrderStatus? (订单状态)
- startDate: DateTime? (开始日期)
- endDate: DateTime? (结束日期)
- orderNumber: string? (订单号)

// 获取订单详情
GET /api/order/{orderId}

// 创建订单
POST /api/order
Content-Type: application/json
{
  "items": [
    {
      "productId": "product-guid-1",
      "quantity": 2
    },
    {
      "productId": "product-guid-2", 
      "quantity": 1
    }
  ],
  "shippingAddress": {
    "receiverName": "张三",
    "receiverPhone": "13800138000",
    "province": "广东省",
    "city": "深圳市",
    "district": "南山区",
    "detailAddress": "科技园路1号",
    "postalCode": "518000"
  },
  "couponCode": "SAVE100",
  "remark": "请尽快发货"
}

// 取消订单
POST /api/order/{orderId}/cancel
Content-Type: application/json
{
  "reason": "用户取消"
}

// 确认收货
POST /api/order/{orderId}/confirm-delivery
```

#### 订单支付接口
```csharp
// 创建支付订单
POST /api/order/{orderId}/payment
Content-Type: application/json
{
  "method": 0,
  "amount": 2999.00
}

// 支付回调
POST /api/order/payment/callback
Content-Type: application/json
{
  "orderId": "order-guid",
  "transactionId": "transaction-id",
  "status": "success",
  "amount": 2999.00
}

// 查询支付状态
GET /api/order/{orderId}/payment/status
```

#### 订单发货接口
```csharp
// 发货
POST /api/order/{orderId}/ship
Content-Type: application/json
{
  "trackingNumber": "SF1234567890",
  "carrier": "顺丰速运",
  "shippingNote": "请轻拿轻放"
}

// 更新物流信息
POST /api/order/shipping/tracking
Content-Type: application/json
{
  "trackingNumber": "SF1234567890",
  "location": "深圳市",
  "status": "运输中",
  "description": "快件已发出"
}
```

### 订单查询接口

#### 用户订单查询
```csharp
// 获取用户订单列表
GET /api/order/my-orders
Query Parameters:
- page: int (页码)
- pageSize: int (每页数量)
- status: OrderStatus? (订单状态)

// 获取用户订单详情
GET /api/order/my-orders/{orderId}
```

#### 管理员订单查询
```csharp
// 获取所有订单列表
GET /api/order/admin/all
Query Parameters:
- page: int (页码)
- pageSize: int (每页数量)
- status: OrderStatus? (订单状态)
- userId: Guid? (用户ID)
- startDate: DateTime? (开始日期)
- endDate: DateTime? (结束日期)

// 获取订单统计
GET /api/order/admin/statistics
Query Parameters:
- startDate: DateTime? (开始日期)
- endDate: DateTime? (结束日期)
```

## 【业务逻辑实现】

### 1. 订单服务实现

#### 订单服务接口
```csharp
public interface IOrderService
{
    Task<PaginatedResult<OrderDto>> GetOrdersAsync(OrderQueryDto query);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
    Task<OrderDto> CreateOrderAsync(CreateOrderCommand command);
    Task<OrderDto> CancelOrderAsync(Guid orderId, string reason);
    Task<OrderDto> ConfirmDeliveryAsync(Guid orderId);
    Task<PaginatedResult<OrderDto>> GetUserOrdersAsync(Guid userId, OrderQueryDto query);
    Task<OrderStatisticsDto> GetOrderStatisticsAsync(OrderStatisticsQueryDto query);
}
```

#### 订单服务实现
```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;
    private readonly IProductRepository productRepository;
    private readonly IUserRepository userRepository;
    private readonly IMediator mediator;
    private readonly ILogger<OrderService> logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUserRepository userRepository,
        IMediator mediator,
        ILogger<OrderService> logger)
    {
        this.orderRepository = orderRepository;
        this.productRepository = productRepository;
        this.userRepository = userRepository;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task<PaginatedResult<OrderDto>> GetOrdersAsync(OrderQueryDto query)
    {
        var orders = await orderRepository.GetOrdersAsync(query);
        var totalCount = await orderRepository.GetOrdersCountAsync(query);
        
        return new PaginatedResult<OrderDto>
        {
            Items = orders.Select(o => o.ToDto()).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        return order?.ToDto();
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderCommand command)
    {
        // 验证用户
        var user = await userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"用户不存在: {command.UserId}");
        }

        // 验证商品和库存
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in command.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"商品不存在: {item.ProductId}");
            }

            if (product.StockQuantity < item.Quantity)
            {
                throw new InvalidOperationException($"商品库存不足: {product.Name}");
            }

            if (product.Status != ProductStatus.Active)
            {
                throw new InvalidOperationException($"商品已下架: {product.Name}");
            }

            var orderItem = new OrderItem(
                item.ProductId,
                product.Name,
                product.SKU,
                product.ProductImages.FirstOrDefault()?.ImageUrl,
                item.Quantity,
                product.Price,
                product.Price * item.Quantity
            );

            orderItems.Add(orderItem);
            totalAmount += orderItem.TotalPrice;
        }

        // 计算优惠
        decimal discountAmount = 0;
        decimal couponDiscount = 0;

        if (!string.IsNullOrEmpty(command.CouponCode))
        {
            // 这里应该调用优惠券服务验证优惠券
            // 暂时使用固定优惠金额
            couponDiscount = 100;
            discountAmount += couponDiscount;
        }

        var finalAmount = totalAmount - discountAmount;

        // 生成订单号
        var orderNumber = GenerateOrderNumber();

        // 创建订单
        var order = new Order(
            orderNumber,
            command.UserId,
            OrderType.Normal,
            totalAmount,
            discountAmount,
            finalAmount,
            command.CouponCode,
            couponDiscount,
            command.Remark
        );

        // 添加订单项
        foreach (var item in orderItems)
        {
            order.AddOrderItem(item);
        }

        // 添加收货地址
        if (command.ShippingAddress != null)
        {
            var shippingAddress = new OrderAddress(
                command.ShippingAddress.ReceiverName,
                command.ShippingAddress.ReceiverPhone,
                command.ShippingAddress.Province,
                command.ShippingAddress.City,
                command.ShippingAddress.District,
                command.ShippingAddress.DetailAddress,
                command.ShippingAddress.PostalCode
            );
            order.SetShippingAddress(shippingAddress);
        }

        // 保存订单
        await orderRepository.AddAsync(order);
        await orderRepository.SaveChangesAsync();

        // 更新商品库存
        foreach (var item in orderItems)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.UpdateStock(product.StockQuantity - item.Quantity);
            }
        }
        await productRepository.SaveChangesAsync();

        logger.LogInformation("订单创建成功: {OrderId}, {OrderNumber}", order.Id, order.OrderNumber);
        return order.ToDto();
    }

    public async Task<OrderDto> CancelOrderAsync(Guid orderId, string reason)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"订单不存在: {orderId}");
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException($"订单状态不允许取消: {order.Status}");
        }

        order.Cancel(reason);
        await orderRepository.SaveChangesAsync();

        // 恢复商品库存
        foreach (var item in order.OrderItems)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.UpdateStock(product.StockQuantity + item.Quantity);
            }
        }
        await productRepository.SaveChangesAsync();

        logger.LogInformation("订单取消成功: {OrderId}, 原因: {Reason}", orderId, reason);
        return order.ToDto();
    }

    public async Task<OrderDto> ConfirmDeliveryAsync(Guid orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"订单不存在: {orderId}");
        }

        if (order.Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException($"订单状态不允许确认收货: {order.Status}");
        }

        order.ConfirmDelivery();
        await orderRepository.SaveChangesAsync();

        logger.LogInformation("订单确认收货成功: {OrderId}", orderId);
        return order.ToDto();
    }

    public async Task<PaginatedResult<OrderDto>> GetUserOrdersAsync(Guid userId, OrderQueryDto query)
    {
        var orders = await orderRepository.GetUserOrdersAsync(userId, query);
        var totalCount = await orderRepository.GetUserOrdersCountAsync(userId, query);
        
        return new PaginatedResult<OrderDto>
        {
            Items = orders.Select(o => o.ToDto()).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(OrderStatisticsQueryDto query)
    {
        var statistics = await orderRepository.GetOrderStatisticsAsync(query);
        return statistics;
    }

    private string GenerateOrderNumber()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmss") + Random.Shared.Next(1000, 9999);
    }
}
```

### 2. 订单状态机实现

#### 订单状态机
```csharp
public class OrderStateMachine
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> ValidTransitions = new()
    {
        { OrderStatus.Pending, new[] { OrderStatus.Paid, OrderStatus.Cancelled } },
        { OrderStatus.Paid, new[] { OrderStatus.Processing, OrderStatus.Refunded } },
        { OrderStatus.Processing, new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
        { OrderStatus.Shipped, new[] { OrderStatus.Delivered, OrderStatus.Cancelled } },
        { OrderStatus.Delivered, new[] { OrderStatus.Completed } },
        { OrderStatus.Completed, new OrderStatus[] { } },
        { OrderStatus.Cancelled, new OrderStatus[] { } },
        { OrderStatus.Refunded, new OrderStatus[] { } }
    };

    public static bool CanTransition(OrderStatus currentStatus, OrderStatus targetStatus)
    {
        if (!ValidTransitions.ContainsKey(currentStatus))
            return false;

        return ValidTransitions[currentStatus].Contains(targetStatus);
    }

    public static void ValidateTransition(OrderStatus currentStatus, OrderStatus targetStatus)
    {
        if (!CanTransition(currentStatus, targetStatus))
        {
            throw new InvalidOperationException(
                $"不允许从状态 {currentStatus} 转换到状态 {targetStatus}");
        }
    }
}
```

## 【使用示例】

### 前端调用示例

#### 创建订单
```javascript
// JavaScript调用
const orderData = {
    items: [
        {
            productId: "product-guid-1",
            quantity: 2
        },
        {
            productId: "product-guid-2",
            quantity: 1
        }
    ],
    shippingAddress: {
        receiverName: "张三",
        receiverPhone: "13800138000",
        province: "广东省",
        city: "深圳市",
        district: "南山区",
        detailAddress: "科技园路1号",
        postalCode: "518000"
    },
    couponCode: "SAVE100",
    remark: "请尽快发货"
};

fetch('/api/order', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(orderData)
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        console.log('订单创建成功:', data.data);
        showSuccessMessage('订单创建成功');
        // 跳转到支付页面
        window.location.href = `/payment?orderId=${data.data.id}`;
    } else {
        console.error('创建失败:', data.message);
        showErrorMessage('订单创建失败: ' + data.message);
    }
});
```

#### 获取用户订单列表
```javascript
// JavaScript调用
const queryParams = new URLSearchParams({
    page: '1',
    pageSize: '10',
    status: '1' // 已支付状态
});

fetch('/api/order/my-orders?' + queryParams.toString(), {
    method: 'GET',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    }
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        console.log('订单列表:', data.data);
        renderOrderList(data.data.items);
        renderOrderPagination(data.data);
    } else {
        console.error('获取失败:', data.message);
    }
});
```

#### 取消订单
```javascript
// JavaScript调用
const cancelData = {
    reason: "用户取消"
};

fetch('/api/order/order-guid-here/cancel', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(cancelData)
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        console.log('订单取消成功');
        showSuccessMessage('订单已取消');
        // 刷新订单列表
        loadOrderList();
    } else {
        console.error('取消失败:', data.message);
        showErrorMessage('订单取消失败: ' + data.message);
    }
});
```

## 【最佳实践】

### 1. 订单状态管理
- **状态机设计**：使用状态机管理订单状态转换
- **状态验证**：确保状态转换的合法性
- **状态记录**：记录状态变化的时间和原因
- **状态通知**：状态变化时通知相关方

### 2. 库存管理
- **库存锁定**：下单时锁定库存
- **库存释放**：取消订单时释放库存
- **库存检查**：下单前检查库存充足性
- **库存更新**：实时更新库存数量

### 3. 支付管理
- **支付状态跟踪**：实时跟踪支付状态
- **支付回调处理**：正确处理支付回调
- **支付超时处理**：处理支付超时情况
- **退款处理**：支持订单退款

### 4. 物流管理
- **物流信息跟踪**：实时跟踪物流信息
- **物流状态更新**：及时更新物流状态
- **配送异常处理**：处理配送异常情况
- **签收确认**：支持签收确认功能

### 5. 数据一致性
- **事务处理**：确保订单操作的原子性
- **状态检查**：操作前检查订单状态
- **并发控制**：防止并发操作导致的数据不一致
- **数据验证**：确保订单数据的完整性

### 6. 性能优化
- **分页查询**：使用分页避免大量数据查询
- **索引优化**：为常用查询字段添加数据库索引
- **缓存策略**：对热门订单数据进行缓存
- **异步处理**：使用异步处理耗时操作

## 【下一步】
接下来我们将学习 **支付服务（Payment Service）**，这是电商平台的核心业务服务。请查看 `08_PaymentService.md` 文档。

---

**学习提示：**
- 订单管理是电商平台的核心，需要完善的状态管理
- 库存管理是避免超卖的关键
- 支付和物流是订单流程的重要环节
- 状态机设计确保订单状态转换的合法性
- 数据一致性对于订单管理至关重要
- 性能优化对于大量订单数据很重要
