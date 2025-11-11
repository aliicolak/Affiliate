using Domain.Entities.UserContent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("usr_wishlist");
            builder.Property(w => w.Name).HasMaxLength(160);
            builder.HasMany(w => w.Items)
                   .WithOne(i => i.Wishlist)
                   .HasForeignKey(i => i.WishlistId);
        }
    }
}
