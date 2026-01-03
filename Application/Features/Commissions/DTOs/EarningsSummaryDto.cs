namespace Application.Features.Commissions.DTOs;

/// <summary>
/// Kazanç özeti
/// </summary>
public sealed class EarningsSummaryDto
{
    /// <summary>
    /// Toplam kazanç (onaylanmış)
    /// </summary>
    public decimal TotalEarnings { get; set; }
    
    /// <summary>
    /// Bekleyen kazanç
    /// </summary>
    public decimal PendingEarnings { get; set; }
    
    /// <summary>
    /// Ödenen toplam
    /// </summary>
    public decimal PaidEarnings { get; set; }
    
    /// <summary>
    /// Çekilebilir bakiye
    /// </summary>
    public decimal AvailableBalance { get; set; }
    
    /// <summary>
    /// Bu dönemin kazancı
    /// </summary>
    public decimal PeriodEarnings { get; set; }
    
    /// <summary>
    /// Son 7 günün kazancı
    /// </summary>
    public decimal Last7DaysEarnings { get; set; }
    
    /// <summary>
    /// Son 30 günün kazancı
    /// </summary>
    public decimal Last30DaysEarnings { get; set; }
    
    /// <summary>
    /// Toplam komisyon sayısı
    /// </summary>
    public int TotalCommissions { get; set; }
    
    /// <summary>
    /// Onaylanan komisyon sayısı
    /// </summary>
    public int ApprovedCommissions { get; set; }
    
    /// <summary>
    /// Para birimi
    /// </summary>
    public string Currency { get; set; } = "TRY";
}
