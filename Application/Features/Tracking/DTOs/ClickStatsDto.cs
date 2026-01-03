namespace Application.Features.Tracking.DTOs;

/// <summary>
/// Tıklama istatistikleri DTO
/// </summary>
public sealed class ClickStatsDto
{
    public int TotalClicks { get; set; }
    public int UniqueClicks { get; set; }
    public int Conversions { get; set; }
    public decimal ConversionRate => TotalClicks > 0 ? (decimal)Conversions / TotalClicks * 100 : 0;
    public List<ClickStatItemDto> Items { get; set; } = new();
    public Dictionary<string, int> ByCountry { get; set; } = new();
    public Dictionary<string, int> ByDevice { get; set; } = new();
}

public sealed class ClickStatItemDto
{
    public DateTime Date { get; set; }
    public int Clicks { get; set; }
    public int UniqueClicks { get; set; }
    public int Conversions { get; set; }
}
