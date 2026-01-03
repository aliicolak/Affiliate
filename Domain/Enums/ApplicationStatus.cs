namespace Domain.Enums;

/// <summary>
/// Satıcı başvuru durumu
/// </summary>
public enum ApplicationStatus
{
    /// <summary>
    /// Taslak
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Gönderildi
    /// </summary>
    Submitted = 1,
    
    /// <summary>
    /// İnceleme altında
    /// </summary>
    UnderReview = 2,
    
    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 3,
    
    /// <summary>
    /// Reddedildi
    /// </summary>
    Rejected = 4
}
