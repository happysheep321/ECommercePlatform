# 🛒 ECommerce 微服务系统

基于 **.NET 8** 与 **DDD（领域驱动设计）** 的微服务电商平台，统一接入 **YARP API Gateway**，实现认证鉴权、服务治理与横向扩展。面向学习与实践微服务架构的 .NET 开发者。

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![C#](https://img.shields.io/badge/C%23-12.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Architecture](https://img.shields.io/badge/Architecture-DDD-orange.svg)](https://martinfowler.com/bliki/DomainDrivenDesign.html)
[![Repo](https://img.shields.io/badge/GitHub-happysheep321%2FECommercePlatform-black?logo=github)](https://github.com/happysheep321/ECommercePlatform)

---

## 📑 目录

- [项目特色](#-项目特色)
- [项目结构](#-项目结构概览)
- [技术栈](#-技术架构)
- [快速启动](#-快速启动)
- [配置说明](#-配置说明)
- [API 规范](#-api-规范)
- [开发指南](#-开发指南)
- [文档导航](#-文档导航)
- [贡献指南](#-贡献指南)
- [License](#-许可证)


---

## 🎯 项目特色

### 🏗️ **DDD 架构设计**
- **领域驱动设计**：按业务领域划分微服务边界
- **聚合根模式**：确保数据一致性和业务规则
- **领域事件**：实现服务间松耦合通信
- **CQRS 模式**：命令查询职责分离

### 🚀 **现代化技术栈**
- **.NET 8**：最新的.NET平台，性能优异
- **YARP 网关**：高性能反向代理，支持动态路由
- **JWT 认证**：无状态身份验证
- **EF Core**：现代化ORM框架
- **Serilog**：结构化日志记录

### 🔧 **通用组件库**
- **BuildingBlocks**：基础设施组件，支持快速开发
- **SharedKernel**：领域模型共享，确保一致性
- **标准化配置**：统一的服务注册和中间件配置

---

## 🧩 项目结构概览

```
ECommercePlatform/
├── 🚪 gateway/                    # API网关层
│   └── ECommerce.APIGateway/     # YARP反向代理网关
├── 🔧 infrastructure/             # 基础设施层
│   ├── ECommerce.BuildingBlocks/ # 通用组件库
│   └── ECommerce.SharedKernel/   # 共享领域模型
├── 🏪 services/                   # 业务服务层
│   ├── identity/                  # 身份认证服务
│   ├── product/                   # 商品管理服务
│   ├── cart/                      # 购物车服务
│   ├── order/                     # 订单管理服务
│   ├── payment/                   # 支付服务
│   └── inventory/                 # 库存管理服务
├── 🧪 tests/                      # 测试项目
└── 📚 docs/                       # 项目文档
```

### 📋 服务说明

| 服务名称 | 端口 | 职责 | 状态 |
|---------|------|------|------|
| `ECommerce.APIGateway` | 5000 | API网关，统一入口 | ✅ 完成 |
| `Ecommerce.Identity.API` | 5001 | 用户认证与授权 | ✅ 完成 |
| `Ecommerce.Product.API` | 5002 | 商品管理 | 🚧 开发中 |
| `Ecommerce.Cart.API` | 5003 | 购物车管理 | 🚧 开发中 |
| `Ecommerce.Order.API` | 5004 | 订单管理 | 🚧 开发中 |
| `Ecommerce.Payment.API` | 5005 | 支付处理 | 🚧 开发中 |
| `Ecommerce.Inventory.API` | 5006 | 库存管理 | 🚧 开发中 |

---

## 🏗️ 技术架构

### 🔧 技术栈详情

| 技术 | 版本 | 用途 | 说明 |
|------|------|------|------|
| **.NET 8** | 8.0 | 运行时 | 最新LTS版本，性能优异 |
| **C# 12** | 12.0 | 开发语言 | 最新语法特性 |
| **YARP** | 2.1.0 | API网关 | 高性能反向代理 |
| **Entity Framework Core** | 9.0.6 | ORM框架 | 数据库访问 |
| **JWT Bearer** | 8.0.17 | 身份认证 | 无状态认证 |
| **Serilog** | 9.0.0 | 日志记录 | 结构化日志 |
| **Swagger** | 6.6.2 | API文档 | 接口文档生成 |
| **MediatR** | 12.5.0 | 中介者模式 | CQRS实现 |
| **FluentValidation** | 12.0.0 | 数据验证 | 输入验证 |
| **Redis** | 2.8.41 | 缓存 | 分布式缓存 |

---

## 🚀 快速启动

### 📋 前置要求

- **.NET 8 SDK** ([下载地址](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server** 或 **SQLite** (开发环境)
- **Redis** (可选，用于缓存)
- **Visual Studio 2022** 或 **VS Code**

### 1️⃣ 克隆项目

```bash
git clone https://github.com/your-username/ECommercePlatform.git
cd ECommercePlatform
```

### 2️⃣ 配置数据库连接

编辑各服务的 `appsettings.json` 文件：

```json
{
  "ConnectionStrings": {
    "UserDb": "Server=localhost;Database=ECommerce.Identity;Trusted_Connection=True;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-minimum-16-characters",
    "Issuer": "ECommercePlatform",
    "Audience": "ECommerceUsers",
    "ExpirationMinutes": 60
  }
}
```

### 3️⃣ 数据库迁移

```bash
# 进入Identity服务目录
cd services/identity/Ecommerce.Identity.API

# 创建数据库迁移
dotnet ef migrations add InitialCreate

# 更新数据库
dotnet ef database update
```

### 4️⃣ 启动服务

#### 方式一：多项目启动（推荐）

1. 在Visual Studio中打开解决方案
2. 右键解决方案 → 设置启动项目
3. 选择"多个启动项目"
4. 设置以下项目为启动：
   - `ECommerce.APIGateway`
   - `Ecommerce.Identity.API`
   - 其他需要的微服务

#### 方式二：命令行启动

```bash
# 启动网关
cd gateway/ECommerce.APIGateway
dotnet run

# 新终端启动Identity服务
cd services/identity/Ecommerce.Identity.API
dotnet run

# 新终端启动其他服务
cd services/cart/ECommerce.Cart.API
dotnet run
```

### 5️⃣ 验证部署

访问以下地址验证服务是否正常：

- **API网关**: http://localhost:5000
- **Identity服务**: http://localhost:5001/swagger
- **Cart服务**: http://localhost:5003/swagger

---

## 🔐 认证与安全

### 🛡️ 安全机制

| 安全特性 | 实现方式 | 说明 |
|---------|---------|------|
| **身份认证** | JWT Bearer Token | 无状态认证，支持分布式部署 |
| **权限控制** | 基于角色的访问控制(RBAC) | 细粒度权限管理 |
| **请求验证** | FluentValidation | 输入数据验证 |
| **服务防护** | X-Gateway-Auth头部 | 防止绕过网关直接访问 |
| **跨域处理** | CORS策略 | 支持前端跨域请求 |

---

## 📡 API接口示例

### 🔐 认证相关

```bash
# 用户注册
POST /identity/api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}

# 用户登录
POST /identity/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Password123!"
}

# 获取用户资料
GET /identity/api/user/profile
Authorization: Bearer <your-jwt-token>
```

### 🛒 业务接口

详见《docs/API接口使用说明.md》。

---

## 🔧 开发指南

### 📁 项目结构规范

```
ServiceName.API/
├── Application/           # 应用层
│   ├── Commands/         # 命令
│   ├── Queries/          # 查询
│   ├── DTOs/             # 数据传输对象
│   ├── Services/         # 应用服务
│   └── Validators/       # 验证器
├── Domain/               # 领域层
│   ├── Aggregates/       # 聚合根
│   ├── Entities/         # 实体
│   ├── ValueObjects/     # 值对象
│   ├── Events/           # 领域事件
│   └── Repositories/     # 仓储接口
├── Infrastructure/       # 基础设施层
│   ├── EntityConfigs/    # 实体配置
│   ├── Repositories/     # 仓储实现
│   └── Services/         # 基础设施服务
└── Controllers/          # 控制器
```

### 🎯 DDD开发原则

1. **领域驱动**: 按业务领域划分服务边界
2. **聚合设计**: 确保数据一致性和业务规则
3. **事件驱动**: 使用领域事件实现松耦合
4. **CQRS模式**: 命令查询职责分离

### 🔄 代码规范

- 使用 **PascalCase** 命名类和方法
- 使用 **camelCase** 命名变量和参数
- 遵循 **SOLID** 原则
- 编写单元测试，保持测试覆盖率 > 80%

---

## 📚 文档导航

| 文档 | 说明 | 链接 |
|------|------|------|
| **通用组件使用指南** | 通用组件库使用说明 | [查看文档](docs/通用组件使用指南.md) |
| **DDD架构简明指南** | 领域驱动设计快速入门 | [查看文档](docs/DDD架构简明指南.md) |
| **API接口使用说明** | 详细的API接口文档 | [查看文档](docs/API接口使用说明.md) |
| **开发规范与最佳实践** | 代码规范和开发指南 | [查看文档](docs/开发规范与最佳实践.md) |

---

## 🤝 贡献指南

### 📝 提交规范

```bash
# 提交格式
<type>(<scope>): <subject>

# 示例
feat(identity): 添加用户注册功能
fix(gateway): 修复JWT验证问题
docs(readme): 更新项目说明文档
```

### 🔄 工作流程

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

---

## 📄 许可证

本项目采用 **MIT** 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

---

## 👥 团队

- **架构师**: [Evan Wen]
- **后端开发**: [Evan Wen]
- **前端开发**: [Evan Wen]
- **测试工程师**: [Evan Wen]

---

## 📞 联系我们

- **邮箱**: 1978691542@qq.com
- **GitHub**: [https://github.com/happysheep321](https://github.com/happysheep321)
- **项目地址**: ([happysheep321/ECommercePlatform: C#分布式电商平台](https://github.com/happysheep321/ECommercePlatform))

---

## ⭐ 如果这个项目对您有帮助，请给我们一个星标！

[![GitHub stars](https://img.shields.io/github/stars/happysheep321/ECommercePlatform.svg?style=social&label=Star)](https://github.com/happysheep321/ECommercePlatform)
[![GitHub forks](https://img.shields.io/github/forks/happysheep321/ECommercePlatform.svg?style=social&label=Fork)](https://github.com/happysheep321/ECommercePlatform)