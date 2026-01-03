using Application.Common.Responses;

namespace Application.Features.Tracking.DTOs;

/// <summary>
/// Sayfalanmış tıklama listesi
/// </summary>
public sealed class PagedClicksDto
{
    public List<ClickItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);
}

public sealed class ClickItemDto
{
    public long Id { get; set; }
    public long OfferId { get; set; }
    public string? OfferName { get; set; }
    public string? MerchantName { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public bool IsConverted { get; set; }
    public DateTime CreatedUtc { get; set; }
}
