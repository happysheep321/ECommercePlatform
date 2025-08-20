# 🛡️ 管理员接口功能规范

## 功能概述

管理员接口是ECommerce平台的核心管理功能模块，为系统管理员提供完整的用户、角色、权限管理能力。本模块遵循DDD架构设计，采用基于Mediator的CQRS模式实现（入门版本），确保系统的安全性、性能和可维护性。

## 功能模块设计

### 1. 用户管理模块

#### 新增接口
- **获取所有用户列表**：`GET /api/user/admin/all`
- **根据ID获取用户详情**：`GET /api/user/admin/{userId}`
- **激活用户账户**：`POST /api/user/{userId}/activate`
- **封禁用户账户**：`POST /api/user/{userId}/ban`
- **冻结用户账户**：`POST /api/user/{userId}/freeze`
- **删除用户账户**：`DELETE /api/user/{userId}`
- **为用户分配角色**：`POST /api/user/role/assign?userId={userId}&roleId={roleId}`
- **移除用户角色**：`DELETE /api/user/role/remove?userId={userId}&roleId={roleId}`

#### 新增文件
- `Application/Commands/GetAllUsersCommand.cs`
- `Application/Commands/GetAllUsersCommandHandler.cs`
- `Application/Commands/GetUserByIdCommand.cs`
- `Application/Commands/GetUserByIdCommandHandler.cs`

#### 修改文件
- `Controllers/UserController.cs` - 添加管理员接口
- `Application/Interfaces/IUserService.cs` - 添加管理员方法
- `Application/Services/UserService.cs` - 实现管理员方法
- `Domain/Repositories/IUserRepository.cs` - 添加GetAllAsync方法
- `Infrastructure/Repositories/UserRepository.cs` - 实现GetAllAsync方法

### 2. 权限管理模块

#### 核心接口
- **获取所有权限列表**：`GET /api/permission`
- **根据ID获取权限详情**：`GET /api/permission/{permissionId}`
- **检查用户是否有指定权限**：`GET /api/permission/check/{userId}/{permissionCode}`

#### 新增文件
- `Controllers/PermissionController.cs` - 权限管理控制器
- `Application/Commands/GetAllPermissionsCommand.cs`
- `Application/Commands/GetAllPermissionsCommandHandler.cs`
- `Application/Services/PermissionService.cs` - 权限服务实现

#### 修改文件
- `Application/Interfaces/IPermissionService.cs` - 权限服务接口
- `Extensions/ServiceExtensions.cs` - 注册权限服务

### 3. 角色管理模块

#### 核心接口
- **获取所有角色列表**：`GET /api/role`
- **根据ID获取角色详情**：`GET /api/role/{roleId}`
- **创建新角色**：`POST /api/role`
- **更新角色信息**：`PUT /api/role`
- **删除角色**：`DELETE /api/role`
- **启用角色**：`POST /api/role/enable`
- **禁用角色**：`POST /api/role/disable`
- **获取角色的权限列表**：`GET /api/role/{roleId}/permissions`
- **为角色分配权限**：`POST /api/role/{roleId}/permissions`

## 技术架构实现

### 1. 权限控制机制
所有管理员接口都使用角色授权：
```csharp
[Authorize(Roles = "Admin")]
```

### 2. 基于Mediator的CQRS架构模式（入门版本）
使用MediatR实现命令查询分离：
- Command：定义操作命令
- Handler：处理命令逻辑
- 通过mediator.Send()发送命令

### 3. 性能优化策略
- **避免N+1查询**：使用Include预加载相关数据，通过ThenInclude预加载关联实体
- **批量查询**：一次性获取所有需要的角色信息
- **AsNoTracking**：对于只读查询使用AsNoTracking提高性能

### 4. 数据一致性保障
- **事务处理**：确保操作的原子性
- **状态检查**：操作前验证数据状态
- **依赖验证**：删除前检查依赖关系

## 开发规范与文档

### 1. 学习文档体系
- `LearningDocs/05_IdentityService.md` - 添加管理员接口功能章节
- `LearningDocs/11_管理员接口实现.md` - 专门的管理员接口学习指南
- `LearningDocs/00_学习指南总览.md` - 更新学习路线图

### 2. 正式文档规范
- `docs/01-architecture.md` - 添加管理员接口模块说明
- `docs/02-development-standards.md` - 添加管理员接口规范

## 测试与验证

### 1. API测试规范
- `ECommerce.Identity.API.http` - 包含所有管理员接口的测试用例

## 系统集成配置

### 1. 依赖注入配置
```csharp
services.AddScoped<IPermissionService, PermissionService>();
```

## 安全与合规

### 1. 权限验证机制
- 所有管理员接口都需要Admin角色
- 使用JWT Token进行身份验证

### 2. 审计日志机制
- 记录所有管理员操作
- 便于审计和问题排查

### 3. 输入验证机制
- 使用FluentValidation进行参数验证
- 防止恶意输入

### 4. 错误处理机制
- 返回用户友好的错误信息
- 不暴露系统内部细节

## 性能优化策略

### 1. 数据库查询优化
- 使用Include预加载相关数据
- 避免N+1查询问题
- 使用AsNoTracking提高只读查询性能

### 2. 缓存优化策略
- 对权限列表等不常变化的数据进行缓存
- 减少数据库访问

### 3. 批量操作优化
- 支持批量用户操作
- 提高操作效率

## 开发最佳实践

### 1. 代码组织规范
- 遵循基于Mediator的CQRS模式（入门版本）
- 使用依赖注入
- 遵循单一职责原则

### 2. 错误处理规范
- 统一的错误响应格式
- 详细的错误日志记录
- 用户友好的错误信息

### 3. 文档维护规范
- 完整的API文档
- 详细的代码注释
- 学习指南和最佳实践

## 总结

管理员接口功能模块是ECommerce平台的核心管理组件，通过DDD架构设计和基于Mediator的CQRS模式实现（入门版本），为系统管理员提供了：

### 🎯 核心能力
- **完整的用户管理**：支持用户的查看、状态管理、角色分配等
- **权限管理**：支持权限的查看、检查等操作
- **角色管理**：支持角色的创建、更新、删除、权限分配等

### 🛡️ 安全保障
- **权限控制**：通过角色授权确保访问安全
- **审计日志**：记录所有管理员操作，便于审计
- **输入验证**：防止恶意输入和攻击

### ⚡ 性能保障
- **查询优化**：避免N+1查询，使用预加载策略
- **缓存机制**：对不常变化的数据进行缓存
- **批量操作**：支持批量处理，提高效率

### 📚 文档体系
- **学习文档**：提供详细的学习指南和最佳实践
- **开发规范**：统一的编码标准和架构规范
- **测试用例**：完整的API测试和验证

该模块为管理员提供了完整的系统管理能力，同时保证了系统的安全性、性能和可维护性，是ECommerce平台不可或缺的核心组件。
