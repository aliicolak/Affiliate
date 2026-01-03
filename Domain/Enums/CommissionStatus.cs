namespace Domain.Enums;

/// <summary>
/// Komisyon durumu
/// </summary>
public enum CommissionStatus
{
    /// <summary>
    /// Onay bekliyor (conversion henüz doğrulanmadı)
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Onaylandı, ödeme bekliyor
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Reddedildi (fraud, iptal vb.)
    /// </summary>
    Rejected = 2,
    
    /// <summary>
    /// Ödendi
    /// </summary>
    Paid = 3,
    
    /// <summary>
    /// İptal edildi (refund vb.)
    /// </summary>
    Reversed = 4
}
