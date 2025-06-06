using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.API.Infrastructure.EntityConfigs
{
    public class UserAddressConfig : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.ToTable("UserAddresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId).IsRequired();

            builder.Property(a => a.ReceiverName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.Region)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Detail)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.IsDefault)
                .IsRequired();
        }
    }
}
