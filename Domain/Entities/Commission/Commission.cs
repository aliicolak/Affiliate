using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Commission;

/// <summary>
/// Kazanılan komisyon kaydı
/// Her conversion için bir komisyon kaydı oluşturulur
/// </summary>
public class Commission : BaseEntity
{
    /// <summary>
    /// Komisyonu kazanan publisher
    /// </summary>
    public long PublisherId { get; set; }
    
    /// <summary>
    /// İlgili tıklama
    /// </summary>
    public long ClickEventId { get; set; }
    
    /// <summary>
    /// İlgili conversion (opsiyonel - henüz oluşturulmamış olabilir)
    /// </summary>
    public long? ConversionId { get; set; }
    
    /// <summary>
    /// Satış tutarı
    /// </summary>
    public decimal SaleAmount { get; set; }
    
    /// <summary>
    /// Komisyon tutarı
    /// </summary>
    public decimal CommissionAmount { get; set; }
    
    /// <summary>
    /// Komisyon oranı (%)
    /// </summary>
    public decimal CommissionRate { get; set; }
    
    /// <summary>
    /// Para birimi
    /// </summary>
    public string Currency { get; set; } = "TRY";
    
    /// <summary>
    /// Komisyon durumu
    /// </summary>
    public CommissionStatus Status { get; set; } = CommissionStatus.Pending;
    
    /// <summary>
    /// Durum değişiklik zamanı
    /// </summary>
    public DateTime? StatusChangedUtc { get; set; }
    
    /// <summary>
    /// Red/iptal sebebi
    /// </summary>
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// Ödeme ID (eğer ödendiyse)
    /// </summary>
    public long? PayoutId { get; set; }
    
    /// <summary>
    /// Merchant notu
    /// </summary>
    public string? MerchantNote { get; set; }
    
    // Navigation
    public Tracking.ClickEvent ClickEvent { get; set; } = null!;
    public Payout? Payout { get; set; }
}
