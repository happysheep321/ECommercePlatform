🛒 ECommerce 微服务系统

> 基于 **.NET 8** 和 **Razor Pages** 构建的电商微服务系统，采用 **YARP API 网关** 架构，支持高可用、高扩展的服务部署与治理。

---

## 🧩 项目结构概览

| 项目名                     | 说明                                         |
| -------------------------- | -------------------------------------------- |
| `ECommerce.APIGateway`     | API 网关，统一路由、认证、跨域、日志注入等   |
| `Ecommerce.Identity.API`   | 用户中心：注册、登录、资料、角色、地址管理等 |
| `Ecommerce.Product.API`    | 商品服务（可扩展）                           |
| `Ecommerce.Cart.API`       | 购物车服务（可扩展）                         |
| `Ecommerce.Order.API`      | 订单服务（可扩展）                           |
| `Ecommerce.Payment.API`    | 支付服务（可扩展）                           |
| `Ecommerce.Inventory.API`  | 库存服务（可扩展）                           |
| `ECommerce.BuildingBlocks` | 通用组件库：认证、日志、异常、常量等         |
| `ECommerce.SharedKernel`   | 领域模型与基础类型共享包                     |

---

## ⚙️ 技术栈

- `.NET 8` / `C# 12`
- Razor Pages（可替换为 Blazor/MVC）
- **YARP**：反向代理 / 动态路由
- **Serilog**：日志记录
- **JWT**：身份认证
- **EF Core**：数据库访问
- **Swagger**：API 文档生成

---

## 🚀 快速启动

### 1️⃣ 配置数据库连接

各服务的 `appsettings.json` 中配置数据库连接字符串，例如：

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ECommerce.Identity;Trusted_Connection=True;"
}
```

------

### 2️⃣ 启动服务（推荐多项目启动）

- 使用 Visual Studio 设置多个启动项目
- 或进入各服务目录，分别运行：

```
dotnet run
```

------

### 3️⃣ 访问 API 网关

默认监听地址：

```
http://localhost:5000/
```

所有请求应通过此地址进入，由网关自动路由到目标服务。

------

## 🔐 认证与安全机制

| 项         | 内容                                       |
| ---------- | ------------------------------------------ |
| 🔑 身份认证 | 使用 JWT，登录成功后返回 Access Token      |
| 🧱 权限控制 | 网关统一校验 Token，有效后转发请求         |
| 🧬 请求注入 | 网关自动添加用户 ID、角色等头部信息        |
| 🔒 服务防护 | 下游服务校验 `X-Gateway-Auth` 防止绕过网关 |

------

## 📌 常用 API 示例（通过网关访问）

| 操作       | 方法与路径                         |
| ---------- | ---------------------------------- |
| 用户注册   | `POST /identity/api/auth/register` |
| 用户登录   | `POST /identity/api/auth/login`    |
| 获取资料   | `GET /identity/api/user/profile`   |
| 添加地址   | `POST /identity/api/user/address`  |
| 查看购物车 | `GET /cart/api/...`                |
| 下单接口   | `POST /order/api/create`           |
| 发起支付   | `POST /payment/api/pay`            |

> ⚠️ 所有请求需携带 `Authorization: Bearer <token>` 头部，且服务前缀不可省略（如 `/identity`, `/order` 等）。

------

## 📊 日志与监控

- 使用 Serilog 输出日志（控制台 / 文件 / 可拓展到 ELK）
- 各服务支持独立日志配置
- 统一日志格式，便于集中收集

------

## 💻 开发建议

- 多项目启动（调试更方便）
- 开发环境下启用 Swagger，访问文档地址如：

```
http://localhost:5001/swagger/index.html
```

- 所有服务均支持热重载与独立调试
- 推荐使用 Postman 进行本地接口联调测试

------

## 🧱 架构设计亮点

- 🌐 API 网关统一入口、安全隔离
- 🧩 各服务松耦合，支持独立部署与扩展
- ♻️ 通用组件复用性强，维护简单
- 🧠 支持后续集成权限系统（RBAC）、限流、熔断等功能

------

## 📦 扩展与定制

- 每个服务可作为独立模块增删替换
- 新服务开发只需：
  1. 创建新 API 项目；
  2. 注册到 `APIGateway` 路由中；
  3. 实现所需逻辑，即可无缝集成。

------

## 📚 更多文档

如需了解详细的 API 数据结构、数据库设计、业务流程图，请参考：

- 各服务 Swagger UI
- 项目源码内注释
- 后续可补充 `docs/` 文件夹用于归档系统设计资料

------

## 🧑‍💻 项目作者

> 如有建议、Bug 或合作意向，欢迎提交 Issue 或 Pull Request！

------

## 🪪 License

MIT License - 仅供学习与研究使用。如需商用请联系作者。