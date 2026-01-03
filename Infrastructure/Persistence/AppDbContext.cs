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
using Domain.Entities.Tracking;
using Domain.Entities.Commission;
using Domain.Entities.Publisher;
using Domain.Entities.Conversion;
using Domain.Entities.Vendor;
using Domain.Entities.Notification;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long,
        IdentityUserClaim<long>, ApplicationUserRole, IdentityUserLogin<long>,
        IdentityRoleClaim<long>, IdentityUserToken<long>>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === DbSets ===
        // Catalog
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CategoryTranslation> CategoryTranslations => Set<CategoryTranslation>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductTranslation> ProductTranslations => Set<ProductTranslation>();
        public DbSet<ProductSpecKey> ProductSpecKeys => Set<ProductSpecKey>();
        public DbSet<ProductSpecValue> ProductSpecValues => Set<ProductSpecValue>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

        // Affiliate
        public DbSet<Merchant> Merchants => Set<Merchant>();
        public DbSet<AffiliateProgram> AffiliatePrograms => Set<AffiliateProgram>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();

        // UserContent
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

        // Tracking
        public DbSet<ClickEvent> ClickEvents => Set<ClickEvent>();
        public DbSet<ClickSession> ClickSessions => Set<ClickSession>();

        // Commission
        public DbSet<Domain.Entities.Commission.Commission> Commissions => Set<Domain.Entities.Commission.Commission>();
        public DbSet<CommissionTier> CommissionTiers => Set<CommissionTier>();
        public DbSet<Payout> Payouts => Set<Payout>();

        // Publisher
        public DbSet<Publisher> Publishers => Set<Publisher>();

        // Conversion
        public DbSet<Domain.Entities.Conversion.Conversion> Conversions => Set<Domain.Entities.Conversion.Conversion>();

        // Vendor
        public DbSet<VendorApplication> VendorApplications => Set<VendorApplication>();

        // Notification
        public DbSet<Domain.Entities.Notification.Notification> Notifications => Set<Domain.Entities.Notification.Notification>();
        public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();

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
            builder.Entity<Brand>().HasQueryFilter(b => b.DeletedUtc == null);
            builder.Entity<Merchant>().HasQueryFilter(m => m.DeletedUtc == null);
            builder.Entity<AffiliateProgram>().HasQueryFilter(a => a.DeletedUtc == null);
            builder.Entity<Offer>().HasQueryFilter(o => o.DeletedUtc == null);

            // Apply configurations dynamically
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

