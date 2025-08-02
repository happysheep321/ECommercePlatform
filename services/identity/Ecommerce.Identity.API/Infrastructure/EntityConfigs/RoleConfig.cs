using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.EntityConfigs
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);
            builder.Property(r=>r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .HasMaxLength(200);

            builder.Property(r => r.IsSystemRole)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.Enabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Metadata
                .FindNavigation(nameof(Role.RolePermissions))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(r => r.RolePermissions)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
