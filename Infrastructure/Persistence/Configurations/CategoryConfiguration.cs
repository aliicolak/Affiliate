using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("catalog_category");
            builder.Property(c => c.Slug).HasMaxLength(160).IsRequired();
            builder.HasIndex(c => c.Slug).IsUnique();

            builder.HasMany(c => c.Children)
                   .WithOne(c => c.Parent)
                   .HasForeignKey(c => c.ParentId);
        }
    }
}
