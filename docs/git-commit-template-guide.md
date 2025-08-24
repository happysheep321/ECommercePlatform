# Git提交消息规范

## 📋 概述

本文档定义了ECommerce平台的Git提交消息规范，确保所有提交都使用统一的格式。

## 📋 提交消息规范

### 格式说明

```
<type>(<scope>): <subject>

<body>

<footer>

### 🎯 模板内可直接使用的示例

#### 新功能示例
```
feat(user): 添加用户头像上传功能

- 支持JPG、PNG格式图片上传
- 自动压缩图片大小
- 添加图片格式验证

Closes #123
```

#### 修复Bug示例
```
fix(order): 修复订单状态更新问题

- 修复订单状态从"待付款"到"已付款"的转换逻辑
- 添加状态转换的验证规则
- 更新相关单元测试

Fixes #456
```

#### 文档更新示例
```
docs(api): 更新用户API文档

- 添加用户注册接口的详细说明
- 更新请求参数和响应格式
- 添加错误码说明

Closes #789
```

#### 代码重构示例
```
refactor(auth): 重构认证中间件

- 将认证逻辑提取到独立的服务类
- 简化中间件的职责
- 提高代码的可测试性

BREAKING CHANGE: 认证中间件的接口发生变化
```

### 📝 模板占位符说明

#### 第一行：`<type>(<scope>): <subject>`
- `<type>`: 提交类型，如 `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `revert`
- `<scope>`: 影响范围，如 `user`, `order`, `admin`, `db`, `api`, `auth`
- `<subject>`: 简短描述，不超过50个字符

#### 空行后：`<body>`
- 详细说明修改原因和具体改动
- 使用列表格式描述
- 解释为什么要这样改

#### 空行后：`<footer>`
- 关联issue：`Closes #123` 或 `Fixes #456`
- 破坏性变更：`BREAKING CHANGE: 描述变更`
- 相关提交：`See also #789`

### 常用类型

| 类型 | 说明 | 示例 |
|------|------|------|
| `feat` | 新功能 | `feat(user): 添加用户注册功能` |
| `fix` | 修复bug | `fix(order): 修复订单状态更新问题` |
| `docs` | 文档修改 | `docs(api): 更新API文档` |
| `style` | 代码格式化 | `style: 统一代码缩进` |
| `refactor` | 代码重构 | `refactor(auth): 重构认证逻辑` |
| `perf` | 性能优化 | `perf(db): 优化数据库查询` |
| `test` | 测试相关 | `test(user): 添加用户服务测试` |
| `chore` | 构建/工具 | `chore: 更新依赖包版本` |
| `revert` | 回滚提交 | `revert: 回滚到上一个版本` |

### 常用Scope

| Scope | 说明 | 示例 |
|-------|------|------|
| `user` | 用户相关 | `feat(user): 添加用户头像功能` |
| `order` | 订单相关 | `fix(order): 修复订单状态问题` |
| `admin` | 管理后台 | `feat(admin): 添加用户管理功能` |
| `db` | 数据库 | `perf(db): 优化查询性能` |
| `api` | API接口 | `docs(api): 更新接口文档` |
| `auth` | 认证授权 | `refactor(auth): 重构认证逻辑` |

### 示例提交消息

#### 新功能
```
feat(user): 添加用户头像上传功能

- 支持JPG、PNG格式图片上传
- 自动压缩图片大小
- 添加图片格式验证

Closes #123
```

#### 修复Bug
```
fix(order): 修复订单状态更新问题

- 修复订单状态从"待付款"到"已付款"的转换逻辑
- 添加状态转换的验证规则
- 更新相关单元测试

Fixes #456
```

#### 文档更新
```
docs(api): 更新用户API文档

- 添加用户注册接口的详细说明
- 更新请求参数和响应格式
- 添加错误码说明

Closes #789
```

#### 代码重构
```
refactor(auth): 重构认证中间件

- 将认证逻辑提取到独立的服务类
- 简化中间件的职责
- 提高代码的可测试性

BREAKING CHANGE: 认证中间件的接口发生变化
```

## 🎯 最佳实践

### 1. 提交消息格式
- **保持简洁**：subject不超过50个字符
- **使用现在时**：使用"add"而不是"added"
- **首字母小写**：不要大写开头
- **不加句号**：不要在subject末尾加句号

### 2. 详细说明
- **详细说明**：在body中详细说明修改原因
- **分点描述**：使用列表格式描述具体改动
- **解释动机**：说明为什么要这样改

### 3. 关联问题
- **关联issue**：在footer中使用`Closes #123`或`Fixes #456`
- **破坏性变更**：使用`BREAKING CHANGE:`标记
- **相关提交**：使用`See also #789`关联相关提交

### 4. 提交频率
- **原子性提交**：每次提交只做一件事
- **及时提交**：完成一个小功能就提交
- **避免大提交**：不要积累太多改动再提交

## 📚 相关资源

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Commit Message Guidelines](https://github.com/angular/angular/blob/main/CONTRIBUTING.md#-commit-message-format)

---

**提示：** 建议团队成员都遵循相同的规范，以保持提交消息的一致性。
