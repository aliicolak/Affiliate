namespace Application.Features.AffiliatePrograms.DTOs;

public sealed class AffiliateProgramDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long MerchantId { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public int DefaultCurrencyId { get; set; }
    public decimal BaseCommissionPct { get; set; }
    public int CookieDays { get; set; }
    public string? TrackingDomain { get; set; }
    public int OfferCount { get; set; }
    public DateTime CreatedUtc { get; set; }
}
