# Docker 容器部署文档

## 一、概述

本文档介绍了电商平台用户微服务所需的 Docker 容器配置，包括 SQL Server 和 Redis 两个服务的启动配置。

------

## 二、环境要求

- Docker Engine >= 20.x
- Docker Compose >= 1.29.x
- 端口要求：
  - SQL Server: 1433
  - Redis: 6379

------

## 三、Docker Compose 配置

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    hostname: sqlserver
    ports:
      - "1433:1433"
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "qingMan!147"  # 请确保密码复杂度符合要求
    volumes:
      - sqlserver_data:/var/opt/mssql
    restart: unless-stopped
    networks:
      - backend

  redis:
    image: redis:7.2
    container_name: redis
    restart: always
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    command: ["redis-server", "--appendonly", "yes"]  # 开启持久化
    networks:
      - backend

volumes:
  sqlserver_data:
  redis_data:

networks:
  backend:
    driver: bridge
```

------

## 四、启动与管理

### 1. 启动容器

```
docker-compose up -d
```

### 2. 查看容器状态

```
docker ps
```

### 3. 停止并删除容器

```
docker-compose down
```

------

## 五、连接配置示例

- 在微服务项目的配置文件 `appsettings.Development.json` 中添加：

```json
{
  "ConnectionStrings": {
    "UserDb": "Server=sqlserver;Database=EcommerceIdentity;User Id=sa;Password=YourStrongPassword;"
  },
  "Redis": {
    "ConnectionString": "redis:6379"
  }
}
```

> 注意：Docker Compose 内的服务互联使用服务名作为主机名。

------

## 六、备注

- 请确保 `MSSQL_SA_PASSWORD` 环境变量符合 SQL Server 密码复杂度要求。
- Redis 配置开启了 AOF 持久化模式，保证数据不丢失。
- 相关数据存储在 Docker 卷中，重启容器不会丢失数据。