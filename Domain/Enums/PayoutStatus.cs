namespace Domain.Enums;

/// <summary>
/// Ödeme talebi durumu
/// </summary>
public enum PayoutStatus
{
    /// <summary>
    /// Talep oluşturuldu
    /// </summary>
    Requested = 0,
    
    /// <summary>
    /// İnceleme aşamasında
    /// </summary>
    UnderReview = 1,
    
    /// <summary>
    /// Onaylandı, işleme alındı
    /// </summary>
    Processing = 2,
    
    /// <summary>
    /// Tamamlandı
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// Reddedildi
    /// </summary>
    Rejected = 4,
    
    /// <summary>
    /// Başarısız (banka hatası vb.)
    /// </summary>
    Failed = 5
}
