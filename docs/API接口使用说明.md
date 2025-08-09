# 📡 API接口使用说明

## 🌐 基础信息

### 网关地址
```
开发环境: http://localhost:5000
```

### 认证方式
```
Authorization: Bearer <JWT_TOKEN>
```

### 请求格式
```
Content-Type: application/json
```

## 🔐 认证接口

### 1. 用户注册
```http
POST /identity/api/auth/register
```

**请求体：**
```json
{
  "username": "testuser",
  "email": "test@example.com", 
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**响应：**
```json
{
  "success": true,
  "data": {
    "userId": "guid",
    "username": "testuser"
  },
  "message": "注册成功"
}
```

### 2. 用户登录
```http
POST /identity/api/auth/login
```

**请求体：**
```json
{
  "username": "testuser",
  "password": "Password123!"
}
```

**响应：**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-01-01T12:00:00Z",
    "user": {
      "id": "guid",
      "username": "testuser",
      "email": "test@example.com"
    }
  },
  "message": "登录成功"
}
```

## 👤 用户管理接口

### 1. 获取用户资料
```http
GET /identity/api/user/profile
Authorization: Bearer <token>
```

**响应：**
```json
{
  "success": true,
  "data": {
    "userId": "guid",
    "username": "testuser",
    "email": "test@example.com",
    "profile": {
      "firstName": "张",
      "lastName": "三",
      "phoneNumber": "13800138000",
      "birthDate": "1990-01-01"
    }
  }
}
```

### 2. 更新用户资料
```http
PUT /identity/api/user/profile
Authorization: Bearer <token>
```

**请求体：**
```json
{
  "firstName": "李",
  "lastName": "四", 
  "phoneNumber": "13900139000",
  "birthDate": "1991-02-02"
}
```

### 3. 添加用户地址
```http
POST /identity/api/user/address
Authorization: Bearer <token>
```

**请求体：**
```json
{
  "street": "北京市朝阳区某某街道123号",
  "city": "北京市",
  "zipCode": "100000",
  "isDefault": true
}
```

### 4. 获取用户地址列表
```http
GET /identity/api/user/addresses
Authorization: Bearer <token>
```

## 🛒 购物车接口

### 1. 获取购物车
```http
GET /cart/api/cart
Authorization: Bearer <token>
```

### 2. 添加商品到购物车
```http
POST /cart/api/cart/items
Authorization: Bearer <token>
```

**请求体：**
```json
{
  "productId": "guid",
  "quantity": 2
}
```

### 3. 更新购物车商品数量
```http
PUT /cart/api/cart/items/{itemId}
Authorization: Bearer <token>
```

**请求体：**
```json
{
  "quantity": 3
}
```

### 4. 删除购物车商品
```http
DELETE /cart/api/cart/items/{itemId}
Authorization: Bearer <token>
```

## 📦 商品接口

### 1. 获取商品列表
```http
GET /product/api/products?page=1&size=10&category=electronics
Authorization: Bearer <token>
```

**查询参数：**
- `page`: 页码（默认1）
- `size`: 每页数量（默认10）
- `category`: 商品分类
- `keyword`: 搜索关键词

### 2. 获取商品详情
```http
GET /product/api/products/{productId}
Authorization: Bearer <token>
```

## 📋 订单接口

### 1. 创建订单
```http
POST /order/api/orders
Authorization: Bearer <token>
```

**请求体：**
```json
{
  "items": [
    {
      "productId": "guid",
      "quantity": 2,
      "price": 99.99
    }
  ],
  "shippingAddress": {
    "street": "北京市朝阳区某某街道123号",
    "city": "北京市", 
    "zipCode": "100000"
  },
  "paymentMethod": "credit_card"
}
```

### 2. 获取订单列表
```http
GET /order/api/orders?page=1&size=10&status=pending
Authorization: Bearer <token>
```

### 3. 获取订单详情
```http
GET /order/api/orders/{orderId}
Authorization: Bearer <token>
```

### 4. 取消订单
```http
POST /order/api/orders/{orderId}/cancel
Authorization: Bearer <token>
```

## 💳 支付接口

### 1. 创建支付
```http
POST /payment/api/payments
Authorization: Bearer <token>
```

**请求体：**
```json
{
  "orderId": "guid",
  "amount": 199.98,
  "paymentMethod": "credit_card",
  "cardInfo": {
    "cardNumber": "4111111111111111",
    "expiryMonth": "12",
    "expiryYear": "2025",
    "cvv": "123"
  }
}
```

### 2. 查询支付状态
```http
GET /payment/api/payments/{paymentId}
Authorization: Bearer <token>
```

## 📊 错误响应格式

### 标准错误响应
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "请求数据验证失败",
    "details": [
      {
        "field": "email",
        "message": "邮箱格式不正确"
      }
    ]
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 常见错误码
- `VALIDATION_ERROR`: 数据验证错误
- `UNAUTHORIZED`: 未授权访问  
- `FORBIDDEN`: 权限不足
- `NOT_FOUND`: 资源不存在
- `INTERNAL_ERROR`: 服务器内部错误

## 🔧 文档与工具（链接）

- Swagger（各服务）：开发环境访问各服务 /swagger
- Postman 集合：请从仓库 docs/tools 目录获取（如需要我可补齐）

## 🚀 快速测试

### 1. 完整流程测试
```bash
# 1. 注册用户
curl -X POST http://localhost:5000/identity/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"Password123!","confirmPassword":"Password123!"}'

# 2. 登录获取token
curl -X POST http://localhost:5000/identity/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Password123!"}'

# 3. 使用token访问受保护资源
curl -X GET http://localhost:5000/identity/api/user/profile \
  -H "Authorization: Bearer <your-token>"
```

### 2. 环境变量
建议设置环境变量：
```bash
export API_BASE_URL=http://localhost:5000
export JWT_TOKEN=your-jwt-token
```

---

**注意事项：**
1. 所有时间戳使用UTC格式
2. 分页从1开始计数
3. 文件上传需要使用multipart/form-data
4. 批量操作建议使用事务确保数据一致性
