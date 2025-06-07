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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
