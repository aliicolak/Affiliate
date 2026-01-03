using Domain.Entities.Affiliate;
using Domain.Entities.Catalog;
using Domain.Entities.Commission;
using Domain.Entities.Conversion;
using Domain.Entities.Notification;
using Domain.Entities.Publisher;
using Domain.Entities.Tracking;
using Domain.Entities.UserContent;
using Domain.Entities.Vendor;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Persistence;

public interface IAppDbContext
{
    // Affiliate
    DbSet<Merchant> Merchants { get; }
    DbSet<AffiliateProgram> AffiliatePrograms { get; }
    DbSet<Offer> Offers { get; }
    DbSet<PriceHistory> PriceHistories { get; }

    // Catalog
    DbSet<Product> Products { get; }
    DbSet<ProductTranslation> ProductTranslations { get; }
    DbSet<Category> Categories { get; }
    DbSet<CategoryTranslation> CategoryTranslations { get; }
    DbSet<Brand> Brands { get; }

    // UserContent
    DbSet<Wishlist> Wishlists { get; }
    DbSet<WishlistItem> WishlistItems { get; }

    // Tracking
    DbSet<ClickEvent> ClickEvents { get; }
    DbSet<ClickSession> ClickSessions { get; }

    // Commission
    DbSet<Domain.Entities.Commission.Commission> Commissions { get; }
    DbSet<CommissionTier> CommissionTiers { get; }
    DbSet<Payout> Payouts { get; }

    // Publisher
    DbSet<Publisher> Publishers { get; }

    // Conversion
    DbSet<Domain.Entities.Conversion.Conversion> Conversions { get; }

    // Vendor
    DbSet<VendorApplication> VendorApplications { get; }

    // Notification
    DbSet<Domain.Entities.Notification.Notification> Notifications { get; }
    DbSet<NotificationTemplate> NotificationTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


