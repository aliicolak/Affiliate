using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("catalog_brand");
            builder.Property(b => b.Name).HasMaxLength(160).IsRequired();
            builder.HasIndex(b => b.Slug).IsUnique();
        }
    }
}
