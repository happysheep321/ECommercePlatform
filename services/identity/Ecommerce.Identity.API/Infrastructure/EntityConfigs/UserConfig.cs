using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.EntityConfigs
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Email)
                .HasMaxLength(200);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(50);

            builder.Property(u => u.Type).IsRequired();
            builder.Property(u => u.Status).IsRequired();

            builder.Property(u => u.RegisterTime).IsRequired();

            // 配置值对象 UserProfile 作为 Owned Entity
            builder.OwnsOne(u => u.Profile, profile =>
            {
                profile.Property(p => p.NickName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Profile_NickName");

                profile.Property(p => p.AvatarUrl)
                    .HasMaxLength(256)
                    .HasColumnName("Profile_AvatarUrl");

                profile.Property(p => p.Birthday)
                    .HasColumnName("Profile_Birthday");

                profile.Property(p => p.Gender)
                    .HasColumnName("Profile_Gender");
            });

            // 配置RoleIds为私有字段集合映射
            builder.Metadata
                .FindNavigation(nameof(User.UserRoles))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.HasMany(u => u.UserRoles)
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId)
               .IsRequired();


            // 配置地址为导航属性
            builder.HasMany(u => u.Addresses)
                .WithOne()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置登录日志关联（如果有导航）
            builder.HasMany<UserLoginLog>()
                .WithOne()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
