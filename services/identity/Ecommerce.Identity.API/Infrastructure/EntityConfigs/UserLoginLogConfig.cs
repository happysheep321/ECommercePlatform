using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.EntityConfigs
{
    public class UserLoginLogConfig : IEntityTypeConfiguration<UserLoginLog>
    {
        public void Configure(EntityTypeBuilder<UserLoginLog> builder)
        {
            builder.ToTable("UserLoginLogs");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.UserId).IsRequired();

            builder.Property(l => l.LoginTime)
                .IsRequired();

            builder.Property(l => l.IpAddress)
                .HasMaxLength(50);

            // 其他字段和索引根据需要添加
        }
    }
}
