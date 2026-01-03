namespace Domain.Enums;

/// <summary>
/// Conversion (dönüşüm) durumu
/// </summary>
public enum ConversionStatus
{
    /// <summary>
    /// Beklemede - henüz doğrulanmadı
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Doğrulandı
    /// </summary>
    Validated = 1,
    
    /// <summary>
    /// Reddedildi
    /// </summary>
    Rejected = 2,
    
    /// <summary>
    /// İptal edildi (refund)
    /// </summary>
    Reversed = 3
}
