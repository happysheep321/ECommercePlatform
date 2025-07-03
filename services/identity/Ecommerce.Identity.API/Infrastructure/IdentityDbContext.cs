using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure
{
    public class IdentityDbContext:DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
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
                new Role(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Admin", "管理员", true),
                new Role(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Seller", "卖家", true),
                new Role(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Buyer", "买家", true),
                new Role(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Guest", "访客", true)
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission(Guid.Parse("aaaa1111-0000-0000-0000-000000000001"), "User.View", "查看用户"),
                new Permission(Guid.Parse("aaaa1111-0000-0000-0000-000000000002"), "User.Edit", "编辑用户"),
                new Permission(Guid.Parse("aaaa1111-0000-0000-0000-000000000003"), "User.Delete", "删除用户"),
                new Permission(Guid.Parse("aaaa1111-0000-0000-0000-000000000004"), "Order.View", "查看订单"),
                new Permission(Guid.Parse("aaaa1111-0000-0000-0000-000000000005"), "Order.Manage", "管理订单")
            );


            modelBuilder.Entity<RolePermission>().HasData(
                // Admin
                new RolePermission
                {
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000001")
                },
                new RolePermission
                {
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000002")
                },
                new RolePermission
                {
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000003")
                },
                new RolePermission
                {
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000004")
                },
                new RolePermission
                {
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000005")
                },

                // Seller
                new RolePermission
                {
                    RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000005")
                },

                // Buyer
                new RolePermission
                {
                    RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    PermissionId = Guid.Parse("aaaa1111-0000-0000-0000-000000000004")
                }
            );
        }
    }
}
