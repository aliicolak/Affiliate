using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Vendor;

/// <summary>
/// Satıcı (Vendor) başvurusu
/// Marketplace'e satıcı olarak başvuru formu
/// </summary>
public class VendorApplication : BaseEntity
{
    /// <summary>
    /// Başvuran kullanıcı
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Şirket/mağaza adı
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// İletişim e-posta
    /// </summary>
    public string ContactEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Telefon
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Website
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Şirket açıklaması
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Ülke
    /// </summary>
    public string CountryCode { get; set; } = "TR";
    
    /// <summary>
    /// Vergi numarası
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Sektör/kategori
    /// </summary>
    public string? Industry { get; set; }
    
    /// <summary>
    /// Tahmini aylık satış hacmi
    /// </summary>
    public string? EstimatedMonthlySales { get; set; }
    
    /// <summary>
    /// Mevcut affiliate ağları
    /// </summary>
    public string? ExistingAffiliateNetworks { get; set; }
    
    /// <summary>
    /// Başvuru durumu
    /// </summary>
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
    
    /// <summary>
    /// Admin notu
    /// </summary>
    public string? AdminNote { get; set; }
    
    /// <summary>
    /// Red sebebi
    /// </summary>
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// İnceleme tarihi
    /// </summary>
    public DateTime? ReviewedUtc { get; set; }
    
    /// <summary>
    /// İnceleyen admin
    /// </summary>
    public long? ReviewedByUserId { get; set; }
    
    /// <summary>
    /// Onay sonrası oluşturulan Merchant ID
    /// </summary>
    public long? CreatedMerchantId { get; set; }
    
    // Navigation
    public Identity.ApplicationUser User { get; set; } = null!;
    public Affiliate.Merchant? CreatedMerchant { get; set; }
}
