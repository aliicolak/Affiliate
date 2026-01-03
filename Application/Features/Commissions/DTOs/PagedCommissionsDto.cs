namespace Application.Features.Commissions.DTOs;

/// <summary>
/// Sayfalanmış komisyon listesi
/// </summary>
public sealed class PagedCommissionsDto
{
    public List<CommissionItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);
}

public sealed class CommissionItemDto
{
    public long Id { get; set; }
    public string? MerchantName { get; set; }
    public string? OfferName { get; set; }
    public decimal SaleAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime? StatusChangedUtc { get; set; }
}
