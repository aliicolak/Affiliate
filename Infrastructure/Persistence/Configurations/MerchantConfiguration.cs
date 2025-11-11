using Domain.Entities.Affiliate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("aff_merchant");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Slug)
                .HasMaxLength(160)
                .IsRequired();

            builder.Property(m => m.Website)
                .HasMaxLength(300);

            builder.HasIndex(m => m.Slug)
                .IsUnique();

            builder.HasIndex(m => m.IsActive);

            // Relations
            builder.HasMany(m => m.Offers)
                   .WithOne(o => o.Merchant)
                   .HasForeignKey(o => o.MerchantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<AffiliateProgram>()
                   .WithOne(p => p.Merchant)
                   .HasForeignKey(p => p.MerchantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
