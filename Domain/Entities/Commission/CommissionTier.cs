using Domain.Common;

namespace Domain.Entities.Commission;

/// <summary>
/// Kademeli komisyon oranları
/// Program bazında farklı tier'lar tanımlanabilir
/// </summary>
public class CommissionTier : BaseEntity
{
    /// <summary>
    /// İlgili affiliate program
    /// </summary>
    public long AffiliateProgramId { get; set; }
    
    /// <summary>
    /// Tier adı
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Minimum satış sayısı (bu tier'a ulaşmak için)
    /// </summary>
    public int MinSalesCount { get; set; }
    
    /// <summary>
    /// Minimum satış tutarı (bu tier'a ulaşmak için)
    /// </summary>
    public decimal MinSalesAmount { get; set; }
    
    /// <summary>
    /// Komisyon oranı (%)
    /// </summary>
    public decimal CommissionRate { get; set; }
    
    /// <summary>
    /// Sabit komisyon tutarı (oran yerine)
    /// </summary>
    public decimal? FixedAmount { get; set; }
    
    /// <summary>
    /// Tier sırası (küçükten büyüğe)
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public Affiliate.AffiliateProgram AffiliateProgram { get; set; } = null!;
}
