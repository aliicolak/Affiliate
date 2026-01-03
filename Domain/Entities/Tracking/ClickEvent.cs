using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Tracking;

/// <summary>
/// Affiliate link tıklama olayı
/// Her tıklama benzersiz olarak kaydedilir
/// </summary>
public class ClickEvent : BaseEntity
{
    /// <summary>
    /// Tıklanan offer
    /// </summary>
    public long OfferId { get; set; }
    
    /// <summary>
    /// Bu tıklamayı yapan publisher (opsiyonel - anonim olabilir)
    /// </summary>
    public long? PublisherId { get; set; }
    
    /// <summary>
    /// Benzersiz tracking kodu
    /// </summary>
    public string TrackingCode { get; set; } = string.Empty;
    
    /// <summary>
    /// IP adresi (hashed veya anonymized olabilir - GDPR)
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Tarayıcı User-Agent
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Referrer URL
    /// </summary>
    public string? Referrer { get; set; }
    
    /// <summary>
    /// Cihaz türü
    /// </summary>
    public DeviceType DeviceType { get; set; } = DeviceType.Unknown;
    
    /// <summary>
    /// Ülke kodu (ISO 3166-1 alpha-2)
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Şehir
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Sub-ID parametreleri (publisher'ın custom tracking'i)
    /// </summary>
    public string? SubId1 { get; set; }
    public string? SubId2 { get; set; }
    public string? SubId3 { get; set; }
    
    /// <summary>
    /// Conversion'a dönüştü mü?
    /// </summary>
    public bool IsConverted { get; set; }
    
    /// <summary>
    /// Conversion ID (eğer dönüştüyse)
    /// </summary>
    public long? ConversionId { get; set; }
    
    // Navigation properties
    public Affiliate.Offer Offer { get; set; } = null!;
}
