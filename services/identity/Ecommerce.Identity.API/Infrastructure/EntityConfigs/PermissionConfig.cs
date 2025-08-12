using ECommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Identity.API.Infrastructure.EntityConfigs
{
    public class PermissionConfig:IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p=>p.Id);
            builder.Property(p=>p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Type)
                .HasConversion<int>() // 枚举转为 int 存储
                .IsRequired();

            builder.Property(p => p.Enabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.Description)
                .HasMaxLength(256);
        }
    }
}
