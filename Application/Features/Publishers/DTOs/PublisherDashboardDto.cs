namespace Application.Features.Publishers.DTOs;

/// <summary>
/// Publisher dashboard özeti
/// </summary>
public sealed class PublisherDashboardDto
{
    public PublisherProfileDto Profile { get; set; } = null!;
    public DashboardStatsDto Stats { get; set; } = null!;
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public sealed class PublisherProfileDto
{
    public long Id { get; set; }
    public string PublisherCode { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? WebsiteUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
}

public sealed class DashboardStatsDto
{
    public int TodayClicks { get; set; }
    public int Last7DaysClicks { get; set; }
    public int TodayConversions { get; set; }
    public decimal Last30DaysEarnings { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal PendingEarnings { get; set; }
    public decimal ConversionRate { get; set; }
}

public sealed class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // Click, Conversion, Payout
    public string Description { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public DateTime CreatedUtc { get; set; }
}
