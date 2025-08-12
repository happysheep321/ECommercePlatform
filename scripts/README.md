# EF Core 迁移管理脚本使用说明

## 概述

本目录包含了用于管理EF Core数据库迁移的PowerShell脚本，可以简化多微服务项目的迁移操作。

## 脚本文件

- `ef-migrations.ps1` - EF Core迁移管理脚本

## 使用方法

### 基本语法

```powershell
# 在项目根目录下执行
.\scripts\ef-migrations.ps1 [服务名称] [操作] [参数]
```

### 📋 推荐执行顺序

#### 开发阶段流程
1. **检查当前状态** → `list` - 查看现有迁移
2. **创建新迁移** → `add` - 添加数据库变更
3. **生成SQL脚本** → `script` - 生成SQL文件（可选）
4. **更新数据库** → `update` - 应用迁移到数据库

#### 生产环境流程
1. **检查当前状态** → `list` - 查看现有迁移
2. **生成SQL脚本** → `script` - 生成SQL文件
3. **手动执行SQL** → 在数据库中执行生成的SQL脚本

#### 撤销操作流程
1. **检查当前状态** → `list` - 查看现有迁移
2. **移除迁移** → `remove` - 删除最后一个迁移（需要数据库连接）

### 参数说明

- `Service` (必需): 服务名称，支持以下值：
  - `identity` - 身份认证服务
  - `product` - 商品服务
  - `cart` - 购物车服务
  - `order` - 订单服务
  - `payment` - 支付服务
  - `inventory` - 库存服务

- `Action` (必需): 操作类型，支持以下值：
  - `add` - 创建新迁移
  - `update` - 更新数据库
  - `remove` - 移除最后一个迁移
  - `script` - 生成SQL脚本
  - `list` - 列出所有迁移

- `MigrationName` (可选): 迁移名称，仅在 `add` 操作时需要
- `OutputPath` (可选): 输出文件路径，仅在 `script` 操作时使用

## 使用示例

### 1. 列出所有迁移（检查当前状态）

```powershell
.\scripts\ef-migrations.ps1 identity list
```

### 2. 为Identity服务创建迁移

```powershell
.\scripts\ef-migrations.ps1 identity add -MigrationName "AddUserProfileFields"
```

### 3. 生成SQL脚本（可选，用于生产环境）

```powershell
.\scripts\ef-migrations.ps1 identity script -OutputPath "migration.sql"
```

### 4. 更新Identity服务数据库

```powershell
.\scripts\ef-migrations.ps1 identity update
```

### 5. 移除最后一个迁移（如果需要撤销）

```powershell
.\scripts\ef-migrations.ps1 identity remove
```

### 6. 为其他服务创建迁移

```powershell
# Product服务
.\scripts\ef-migrations.ps1 product add -MigrationName "InitialProductSchema"

# Cart服务
.\scripts\ef-migrations.ps1 cart add -MigrationName "InitialCartSchema"

# Order服务
.\scripts\ef-migrations.ps1 order add -MigrationName "InitialOrderSchema"
```

## 注意事项

1. **执行位置**: 脚本需要在项目根目录下执行（不是scripts目录）
2. **权限要求**: 确保有足够的权限执行dotnet命令
3. **服务配置**: 确保目标服务已正确配置EF Core
4. **数据库连接**: 确保数据库连接字符串配置正确
5. **编码问题**: 脚本使用英文输出，避免中文编码问题

## 错误处理

脚本包含以下错误处理机制：

- 检查服务名称是否有效
- 验证项目路径是否存在
- 检查必需参数是否提供
- 捕获并显示命令执行错误

## 支持的微服务

| 服务名称 | 项目路径 | 状态 |
|---------|---------|------|
| identity | services/identity/Ecommerce.Identity.API | ✅ 已配置 |
| product | services/product/ECommerce.Product.API | 🚧 需要配置 |
| cart | services/cart/ECommerce.Cart.API | 🚧 需要配置 |
| order | services/order/ECommerce.Order.API | 🚧 需要配置 |
| payment | services/payment/ECommerce.Payment.API | 🚧 需要配置 |
| inventory | services/inventory/ECommerce.Inventory.API | 🚧 需要配置 |

## 快速参考

### 常见场景

#### 新功能开发
```powershell
# 1. 检查当前状态
.\scripts\ef-migrations.ps1 identity list

# 2. 创建功能迁移
.\scripts\ef-migrations.ps1 identity add -MigrationName "AddUserProfileFields"

# 3. 更新数据库
.\scripts\ef-migrations.ps1 identity update
```

#### 生产部署
```powershell
# 1. 生成SQL脚本
.\scripts\ef-migrations.ps1 identity script -OutputPath "production-migration.sql"

# 2. 在数据库中执行生成的SQL脚本
```

#### 撤销错误迁移
```powershell
# 1. 移除最后一个迁移
.\scripts\ef-migrations.ps1 identity remove

# 2. 重新创建正确的迁移
.\scripts\ef-migrations.ps1 identity add -MigrationName "CorrectMigrationName"
```

## 扩展说明

如需添加新的微服务支持，请修改 `ef-migrations.ps1` 文件中的 `$Services` 哈希表：

```powershell
$Services = @{
    "newservice" = @{
        Path = "services/newservice/ECommerce.NewService.API"
        StartupPath = "services/newservice/ECommerce.NewService.API"
    }
    # ... 其他服务
}
```
