using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Events;
using Ecommerce.SharedKernel.Interfaces;
using ECommerce.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Identity.API.Infrastructure
{
        public class IdentityDbContext : DbContext
        {
            private readonly IDomainEventDispatcher? domainEventDispatcher;

            public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IDomainEventDispatcher domainEventDispatcher) : base(options)
            {
                this.domainEventDispatcher = domainEventDispatcher;
            }

            public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
            {
            }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                var result = await base.SaveChangesAsync(cancellationToken);

                // 获取所有继承自EntityBase<TId>的实体，并收集它们的领域事件
                var entitiesWithEvents = ChangeTracker.Entries<EntityBase<Guid>>()
                    .Select(e => e.Entity)
                    .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                    .ToArray();

                // 派发所有领域事件
                foreach (var entity in entitiesWithEvents)
                {
                    var events = entity.DomainEvents!.ToArray();
                    foreach (var domainEvent in events)
                    {
                        await domainEventDispatcher!.DispatchAsync(domainEvent);
                    }
                    entity.ClearDomainEvents();
                }

                return result;
            }

            public DbSet<User> Users { get; set; }

            public DbSet<UserAddress> UserAddresses { get; set; }

            public DbSet<UserLoginLog> UserLoginLogs { get; set; }

            public DbSet<UserRole> UserRoles { get; set; }

            public DbSet<Role> Roles { get; set; }

            public DbSet<RolePermission> RolePermissions { get; set; }

            public DbSet<Permission> Permissions { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

                // 角色基础数据
                modelBuilder.Entity<Role>().HasData(
                    new
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "Admin",
                        Description = "管理员",
                        Enabled = true,
                        IsSystemRole = true
                    },
                    new
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Name = "Seller",
                        Description = "卖家",
                        Enabled = true,
                        IsSystemRole = true
                    },
                    new
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Name = "Buyer",
                        Description = "买家",
                        Enabled = true,
                        IsSystemRole = true
                    },
                    new
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Name = "Guest",
                        Description = "访客",
                        Enabled = true,
                        IsSystemRole = true
                    }
                );

                // 权限基础数据
                modelBuilder.Entity<Permission>().HasData(
                    new
                    {
                        Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000001"),
                        Name = "Page:User.View",
                        DisplayName = "用户管理页面",
                        Type = (int)PermissionType.Page, // 假设PermissionType是枚举，这里转换成int
                        Enabled = true,
                        Description = "用户管理页面"
                    },
                    new
                    {
                        Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000004"),
                        Name = "Page:Order.View",
                        DisplayName = "订单管理页面",
                        Type = (int)PermissionType.Page,
                        Enabled = true,
                        Description = "订单管理页面"
                    },
                    new
                    {
                        Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000002"),
                        Name = "Permission:User.Edit",
                        DisplayName = "编辑用户",
                        Type = (int)PermissionType.Function,
                        Enabled = true,
                        Description = "编辑用户"
                    },
                    new
                    {
                        Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000003"),
                        Name = "Permission:User.Delete",
                        DisplayName = "删除用户",
                        Type = (int)PermissionType.Function,
                        Enabled = true,
                        Description = "删除用户"
                    },
                    new
                    {
                        Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000005"),
                        Name = "Permission:Order.Manage",
                        DisplayName = "管理订单",
                        Type = (int)PermissionType.Function,
                        Enabled = true,
                        Description = "管理订单"
                    }
                );



                modelBuilder.Entity<RolePermission>().HasData(
                    // Admin 拥有所有权限
                    new { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000001") },
                    new { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000002") },
                    new { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000003") },
                    new { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000004") },
                    new { RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000005") },

                    // Seller 只管理订单权限
                    new { RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000005") },

                    // Buyer 只查看订单权限
                    new { RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333"), PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000004") }
                );
            }
        }
}
