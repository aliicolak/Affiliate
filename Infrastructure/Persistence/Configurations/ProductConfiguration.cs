using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("catalog_product");
            builder.Property(p => p.Slug).HasMaxLength(160).IsRequired();
            builder.HasIndex(p => p.Slug).IsUnique();
            builder.HasIndex(p => p.IsActive);

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

            builder.HasMany(p => p.SpecValues)
                .WithOne(sv => sv.Product)
                .HasForeignKey(sv => sv.ProductId);
        }
    }
}
