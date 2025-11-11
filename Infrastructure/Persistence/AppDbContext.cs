using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Affiliate;
using Domain.Entities.Catalog;
using Domain.Entities.Identity;
using Domain.Entities.UserContent;
using Domain.Entities;
using Domain.Common;
using Application.Abstractions.Persistence;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long,
        IdentityUserClaim<long>, ApplicationUserRole, IdentityUserLogin<long>,
        IdentityRoleClaim<long>, IdentityUserToken<long>>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === DbSets ===
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CategoryTranslation> CategoryTranslations => Set<CategoryTranslation>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductTranslation> ProductTranslations => Set<ProductTranslation>();
        public DbSet<ProductSpecKey> ProductSpecKeys => Set<ProductSpecKey>();
        public DbSet<ProductSpecValue> ProductSpecValues => Set<ProductSpecValue>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Merchant> Merchants => Set<Merchant>();
        public DbSet<AffiliateProgram> AffiliatePrograms => Set<AffiliateProgram>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
        public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity tablo isimlerini özelleştiriyoruz
            builder.Entity<ApplicationUser>().ToTable("app_user");
            builder.Entity<ApplicationRole>().ToTable("app_role");
            builder.Entity<ApplicationUserRole>().ToTable("app_user_role");
            builder.Entity<IdentityUserClaim<long>>().ToTable("app_user_claim");
            builder.Entity<IdentityUserLogin<long>>().ToTable("app_user_login");
            builder.Entity<IdentityUserToken<long>>().ToTable("app_user_token");
            builder.Entity<IdentityRoleClaim<long>>().ToTable("app_role_claim");

            // Composite Keys
            builder.Entity<ProductSpecValue>().HasKey(x => new { x.ProductId, x.SpecKeyId });
            builder.Entity<WishlistItem>().HasKey(x => new { x.WishlistId, x.ProductId });
            builder.Entity<ApplicationUserRole>().HasKey(x => new { x.UserId, x.RoleId });

            // Global Query Filters (soft delete)
            builder.Entity<Product>().HasQueryFilter(p => p.DeletedUtc == null);
            builder.Entity<Category>().HasQueryFilter(c => c.DeletedUtc == null);

            // Apply configurations dynamically
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
