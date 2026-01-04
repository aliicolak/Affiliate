namespace Domain.Enums;

/// <summary>
/// Bildirim türü
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Genel bilgi
    /// </summary>
    Info = 0,
    
    /// <summary>
    /// Başarı bildirimi
    /// </summary>
    Success = 1,
    
    /// <summary>
    /// Uyarı
    /// </summary>
    Warning = 2,
    
    /// <summary>
    /// Hata
    /// </summary>
    Error = 3,
    
    /// <summary>
    /// Komisyon kazanıldı
    /// </summary>
    CommissionEarned = 10,
    
    /// <summary>
    /// Ödeme işlendi
    /// </summary>
    PayoutProcessed = 11,
    
    /// <summary>
    /// Başvuru güncellendi
    /// </summary>
    ApplicationStatusChanged = 12,
    
    /// <summary>
    /// Yeni teklif
    /// </summary>
    NewOffer = 13,

    // Social
    SocialLike = 20,
    SocialComment = 21,
    SocialFollow = 22,
    SocialShare = 23
}
