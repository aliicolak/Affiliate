using Domain.Entities.Affiliate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.ToTable("aff_offer");
            builder.Property(o => o.AffiliateUrl).HasMaxLength(700);
            builder.HasIndex(o => new { o.ProductId, o.InStock });
            builder.HasOne(o => o.Merchant).WithMany(m => m.Offers).HasForeignKey(o => o.MerchantId);
        }
    }
}
