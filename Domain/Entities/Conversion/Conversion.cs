using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Conversion;

/// <summary>
/// Satış/dönüşüm kaydı
/// Merchant'tan gelen conversion bildirimi
/// </summary>
public class Conversion : BaseEntity
{
    /// <summary>
    /// İlgili tıklama
    /// </summary>
    public long ClickEventId { get; set; }
    
    /// <summary>
    /// Merchant tarafından sağlanan sipariş/transaction ID
    /// </summary>
    public string ExternalOrderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Satış tutarı
    /// </summary>
    public decimal SaleAmount { get; set; }
    
    /// <summary>
    /// Para birimi
    /// </summary>
    public string Currency { get; set; } = "TRY";
    
    /// <summary>
    /// Conversion durumu
    /// </summary>
    public ConversionStatus Status { get; set; } = ConversionStatus.Pending;
    
    /// <summary>
    /// Ürün bilgisi (opsiyonel)
    /// </summary>
    public string? ProductInfo { get; set; }
    
    /// <summary>
    /// Müşteri ID (privacy için hash'lenmiş)
    /// </summary>
    public string? CustomerHash { get; set; }
    
    /// <summary>
    /// Doğrulama zamanı
    /// </summary>
    public DateTime? ValidatedUtc { get; set; }
    
    /// <summary>
    /// Red/iptal sebebi
    /// </summary>
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// Postback raw data (debug için)
    /// </summary>
    public string? RawPostback { get; set; }
    
    // Navigation
    public Tracking.ClickEvent ClickEvent { get; set; } = null!;
}
