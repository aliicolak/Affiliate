namespace Domain.Enums;

/// <summary>
/// Publisher durumu
/// </summary>
public enum PublisherStatus
{
    /// <summary>
    /// Başvuru bekliyor / Profil eksik
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Aktif - Affiliate linkleri oluşturabilir
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Askıya alınmış (geçici)
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// Yasaklanmış (kalıcı)
    /// </summary>
    Banned = 3
}
