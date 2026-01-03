using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Publisher;

/// <summary>
/// Publisher profili - affiliate yayıncı bilgileri
/// ApplicationUser'a bağlı ek profil
/// </summary>
public class Publisher : BaseEntity
{
    /// <summary>
    /// İlişkili kullanıcı
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Publisher durumu
    /// </summary>
    public PublisherStatus Status { get; set; } = PublisherStatus.Pending;
    
    /// <summary>
    /// Benzersiz publisher kodu
    /// </summary>
    public string PublisherCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Şirket/site adı
    /// </summary>
    public string? CompanyName { get; set; }
    
    /// <summary>
    /// Website URL
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Promosyon yöntemleri (virgülle ayrılmış)
    /// </summary>
    public string? PromotionMethods { get; set; }
    
    /// <summary>
    /// Vergi numarası
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Ülke kodu
    /// </summary>
    public string CountryCode { get; set; } = "TR";
    
    /// <summary>
    /// Tercih edilen ödeme yöntemi
    /// </summary>
    public string PreferredPaymentMethod { get; set; } = "Bank";
    
    /// <summary>
    /// Ödeme bilgileri (JSON)
    /// </summary>
    public string? PaymentDetails { get; set; }
    
    /// <summary>
    /// Onay tarihi
    /// </summary>
    public DateTime? ApprovedUtc { get; set; }
    
    /// <summary>
    /// Son aktivite
    /// </summary>
    public DateTime LastActivityUtc { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Identity.ApplicationUser User { get; set; } = null!;
}
