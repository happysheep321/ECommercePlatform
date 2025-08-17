# 04_APIGateway - API网关学习指南

## 【学习目标】
通过本文档，您将学会：
- 理解什么是API网关及其作用
- 掌握YARP反向代理的工作原理
- 学会配置路由规则和集群
- 理解网关的认证授权机制
- 能够独立配置和管理API网关

## 【什么是API网关？】

### 概念解释
API网关就像是一个**智能门卫**，负责管理所有进入系统的请求：
- **统一入口**：所有客户端请求都从这里进入
- **路由转发**：把请求转发到对应的微服务
- **认证授权**：检查用户身份和权限
- **限流熔断**：保护系统不被过载
- **日志监控**：记录所有请求信息

### 为什么需要API网关？
想象一下，如果没有API网关：
- 客户端需要知道每个微服务的地址
- 每个微服务都要处理认证授权
- 跨域问题难以解决
- 无法统一管理请求

API网关解决了这些问题：
- **简化客户端**：客户端只需要知道网关地址
- **统一认证**：在网关层统一处理认证
- **集中管理**：统一的路由、限流、监控

## 【YARP简介】

### 什么是YARP？
YARP（Yet Another Reverse Proxy）是微软开发的高性能反向代理：
- **高性能**：基于.NET 8，性能优异
- **可扩展**：支持自定义中间件
- **配置灵活**：支持动态配置
- **易于使用**：集成简单，文档完善

### YARP vs 其他网关
| 特性 | YARP | Nginx | Kong | Zuul |
|------|------|-------|------|------|
| 性能 | 高 | 高 | 中 | 中 |
| 配置复杂度 | 低 | 中 | 高 | 中 |
| .NET集成 | 完美 | 一般 | 一般 | 一般 |
| 学习成本 | 低 | 中 | 高 | 中 |

## 【项目结构详解】

### 核心文件
```
ECommerce.APIGateway/
├── Program.cs                    # 程序入口
├── appsettings.json             # 基础配置
├── appsettings.Development.json # 开发环境配置
└── ECommerce.APIGateway.csproj  # 项目文件
```

### 配置结构
```json
{
  "ReverseProxy": {
    "Routes": {      // 路由规则
      "route_name": {
        "ClusterId": "cluster_name",
        "Match": { "Path": "/path" },
        "Transforms": [...],
        "AuthorizationPolicy": "*"
      }
    },
    "Clusters": {    // 服务集群
      "cluster_name": {
        "Destinations": {
          "d1": { "Address": "http://localhost:port/" }
        }
      }
    }
  }
}
```

## 【核心功能详解】

### 1. 路由配置

#### 什么是路由？
路由就像**邮递员的分拣规则**：
- 根据请求路径决定发送到哪个服务
- 支持路径匹配和转换
- 可以添加请求头和处理逻辑

#### 路由配置示例
```json
{
  "identity_auth_login": {
    "ClusterId": "identity",
    "Match": { 
      "Path": "/identity/api/auth/login" 
    },
    "Transforms": [
      { "PathRemovePrefix": "/identity" },
      {
        "RequestHeader": "X-Gateway-Auth",
        "Set": "true"
      }
    ]
  }
}
```

#### 配置解释
- **`identity_auth_login`**：路由名称
- **`ClusterId: "identity"`**：目标集群
- **`Match.Path`**：匹配的路径模式
- **`Transforms`**：请求转换规则
  - `PathRemovePrefix: "/identity"`：移除路径前缀
  - `RequestHeader`：添加请求头

### 2. 集群配置

#### 什么是集群？
集群就像**服务组**：
- 包含一个或多个相同服务的实例
- 支持负载均衡
- 可以配置健康检查

#### 集群配置示例
```json
{
  "identity": {
    "Destinations": {
      "d1": {
        "Address": "http://localhost:5101/"
      }
    }
  },
  "product": {
    "Destinations": {
      "d1": {
        "Address": "http://localhost:5102/"
      }
    }
  }
}
```

#### 多实例配置示例
```json
{
  "identity": {
    "Destinations": {
      "d1": { "Address": "http://localhost:5101/" },
      "d2": { "Address": "http://localhost:5102/" },
      "d3": { "Address": "http://localhost:5103/" }
    },
    "LoadBalancingPolicy": "RoundRobin"
  }
}
```

### 3. 请求转换

#### 路径转换
```json
{
  "Transforms": [
    { "PathRemovePrefix": "/identity" },     // 移除前缀
    { "PathSet": "/api/auth/login" },        // 设置新路径
    { "PathPattern": "/api/{**catch-all}" }  // 路径模式
  ]
}
```

#### 请求头转换
```json
{
  "Transforms": [
    {
      "RequestHeader": "X-Gateway-Auth",
      "Set": "true"
    },
    {
      "RequestHeader": "X-User-Id",
      "FromRoute": "userId"
    },
    {
      "RequestHeader": "Authorization",
      "Append": "Bearer "
    }
  ]
}
```

### 4. 认证授权

#### 认证配置
```json
{
  "identity_protected": {
    "ClusterId": "identity",
    "Match": { "Path": "/identity/api/user/{**catch-all}" },
    "AuthorizationPolicy": "*",  // 需要认证
    "Transforms": [
      { "PathRemovePrefix": "/identity" }
    ]
  }
}
```

#### 无需认证的路由
```json
{
  "identity_auth_register": {
    "ClusterId": "identity",
    "Match": { "Path": "/identity/api/auth/register" },
    // 没有 AuthorizationPolicy，表示无需认证
    "Transforms": [
      { "PathRemovePrefix": "/identity" }
    ]
  }
}
```

## 【实际配置示例】

### 完整的网关配置
```json
{
  "ReverseProxy": {
    "Routes": {
      // 身份认证服务 - 无需认证
      "identity_auth_register": {
        "ClusterId": "identity",
        "Match": { "Path": "/identity/api/auth/register" },
        "Transforms": [
          { "PathRemovePrefix": "/identity" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      "identity_auth_login": {
        "ClusterId": "identity",
        "Match": { "Path": "/identity/api/auth/login" },
        "Transforms": [
          { "PathRemovePrefix": "/identity" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      
      // 身份认证服务 - 需要认证
      "identity_protected": {
        "ClusterId": "identity",
        "Match": { "Path": "/identity/api/user/{**catch-all}" },
        "AuthorizationPolicy": "*",
        "Transforms": [
          { "PathRemovePrefix": "/identity" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      
      // 商品服务
      "product": {
        "ClusterId": "product",
        "Match": { "Path": "/product/{**catch-all}" },
        "Transforms": [
          { "PathRemovePrefix": "/product" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      
      // 购物车服务
      "cart": {
        "ClusterId": "cart",
        "Match": { "Path": "/cart/{**catch-all}" },
        "AuthorizationPolicy": "*",
        "Transforms": [
          { "PathRemovePrefix": "/cart" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      
      // 订单服务
      "order": {
        "ClusterId": "order",
        "Match": { "Path": "/order/{**catch-all}" },
        "AuthorizationPolicy": "*",
        "Transforms": [
          { "PathRemovePrefix": "/order" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      
      // 支付服务
      "payment": {
        "ClusterId": "payment",
        "Match": { "Path": "/payment/{**catch-all}" },
        "AuthorizationPolicy": "*",
        "Transforms": [
          { "PathRemovePrefix": "/payment" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      },
      
      // 库存服务
      "inventory": {
        "ClusterId": "inventory",
        "Match": { "Path": "/inventory/{**catch-all}" },
        "Transforms": [
          { "PathRemovePrefix": "/inventory" },
          { "RequestHeader": "X-Gateway-Auth", "Set": "true" }
        ]
      }
    },
    
    "Clusters": {
      "identity": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5101/" }
        }
      },
      "product": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5102/" }
        }
      },
      "cart": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5103/" }
        }
      },
      "order": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5104/" }
        }
      },
      "payment": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5105/" }
        }
      },
      "inventory": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5106/" }
        }
      }
    }
  }
}
```

## 【请求流程详解】

### 1. 用户登录流程
```
客户端请求: POST /identity/api/auth/login
    ↓
网关接收请求
    ↓
匹配路由: identity_auth_login
    ↓
路径转换: /identity/api/auth/login → /api/auth/login
    ↓
添加请求头: X-Gateway-Auth: true
    ↓
转发到: http://localhost:5101/api/auth/login
    ↓
身份服务处理登录
    ↓
返回JWT Token
    ↓
网关返回响应给客户端
```

### 2. 获取用户信息流程
```
客户端请求: GET /identity/api/user/profile
    ↓
网关接收请求
    ↓
检查Authorization头中的JWT Token
    ↓
验证Token有效性
    ↓
匹配路由: identity_protected
    ↓
路径转换: /identity/api/user/profile → /api/user/profile
    ↓
添加请求头: X-Gateway-Auth: true
    ↓
转发到: http://localhost:5101/api/user/profile
    ↓
身份服务返回用户信息
    ↓
网关返回响应给客户端
```

### 3. 创建订单流程
```
客户端请求: POST /order/api/orders
    ↓
网关接收请求
    ↓
检查Authorization头中的JWT Token
    ↓
验证Token有效性
    ↓
匹配路由: order
    ↓
路径转换: /order/api/orders → /api/orders
    ↓
添加请求头: X-Gateway-Auth: true
    ↓
转发到: http://localhost:5104/api/orders
    ↓
订单服务创建订单
    ↓
网关返回响应给客户端
```

## 【高级功能】

### 1. 负载均衡
```json
{
  "identity": {
    "Destinations": {
      "d1": { "Address": "http://localhost:5101/" },
      "d2": { "Address": "http://localhost:5102/" },
      "d3": { "Address": "http://localhost:5103/" }
    },
    "LoadBalancingPolicy": "RoundRobin"
  }
}
```

### 2. 健康检查
```json
{
  "identity": {
    "Destinations": {
      "d1": { "Address": "http://localhost:5101/" }
    },
    "HealthCheck": {
      "Active": {
        "Enabled": true,
        "Interval": "00:00:10",
        "Timeout": "00:00:10",
        "Policy": "ConsecutiveFailures",
        "Path": "/health"
      }
    }
  }
}
```

### 3. 限流配置
```json
{
  "identity_auth_login": {
    "ClusterId": "identity",
    "Match": { "Path": "/identity/api/auth/login" },
    "RateLimiterPolicy": "fixed",
    "Transforms": [
      { "PathRemovePrefix": "/identity" }
    ]
  }
}
```

## 【监控和日志】

### 日志配置
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "Enrich": ["FromLogContext"],
    "WriteTo": [
      {
        "Name": "Console"
      }
    ]
  }
}
```

### 监控指标
- 请求数量
- 响应时间
- 错误率
- 服务健康状态
- 路由匹配情况

## 【下一步】
接下来我们将学习 **身份认证服务（Identity Service）**，这是整个系统的用户管理和认证中心。请查看 `05_IdentityService.md` 文档。

---

**学习提示：**
- API网关是微服务架构的关键组件，理解其工作原理很重要
- 路由配置要仔细规划，确保路径匹配正确
- 认证授权配置要安全合理，避免权限漏洞
- 监控和日志对于排查问题非常重要

