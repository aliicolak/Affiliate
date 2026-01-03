using Application.Abstractions.Persistence;
using Application.Features.Publishers.DTOs;
using Application.Features.Publishers.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Handlers;

/// <summary>
/// Publisher dashboard handler'ı
/// </summary>
public sealed class GetPublisherDashboardHandler 
    : IRequestHandler<GetPublisherDashboardQuery, PublisherDashboardDto>
{
    private readonly IAppDbContext _context;

    public GetPublisherDashboardHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PublisherDashboardDto> Handle(
        GetPublisherDashboardQuery request, 
        CancellationToken cancellationToken)
    {
        var publisher = await _context.Publishers
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (publisher is null)
            throw new InvalidOperationException("Publisher profile not found");

        var now = DateTime.UtcNow;
        var today = now.Date;
        var last7Days = today.AddDays(-7);
        var last30Days = today.AddDays(-30);

        // Tıklama istatistikleri
        var clicksQuery = _context.ClickEvents
            .Where(c => c.PublisherId == publisher.Id);

        var todayClicks = await clicksQuery
            .CountAsync(c => c.CreatedUtc >= today, cancellationToken);

        var last7DaysClicks = await clicksQuery
            .CountAsync(c => c.CreatedUtc >= last7Days, cancellationToken);

        var todayConversions = await clicksQuery
            .CountAsync(c => c.CreatedUtc >= today && c.IsConverted, cancellationToken);

        // Kazanç istatistikleri
        var commissionsQuery = _context.Commissions
            .Where(c => c.PublisherId == publisher.Id);

        var last30DaysEarnings = await commissionsQuery
            .Where(c => c.CreatedUtc >= last30Days && 
                       (c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid))
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        var availableBalance = await commissionsQuery
            .Where(c => c.Status == CommissionStatus.Approved && c.PayoutId == null)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        var pendingEarnings = await commissionsQuery
            .Where(c => c.Status == CommissionStatus.Pending)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Son aktiviteler
        var recentClicks = await clicksQuery
            .OrderByDescending(c => c.CreatedUtc)
            .Take(5)
            .Select(c => new RecentActivityDto
            {
                Type = c.IsConverted ? "Conversion" : "Click",
                Description = c.Offer.Merchant.Name,
                CreatedUtc = c.CreatedUtc
            })
            .ToListAsync(cancellationToken);

        var recentCommissions = await commissionsQuery
            .Where(c => c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid)
            .OrderByDescending(c => c.StatusChangedUtc)
            .Take(5)
            .Select(c => new RecentActivityDto
            {
                Type = "Commission",
                Description = $"Commission {c.Status}",
                Amount = c.CommissionAmount,
                CreatedUtc = c.StatusChangedUtc ?? c.CreatedUtc
            })
            .ToListAsync(cancellationToken);

        var recentActivities = recentClicks
            .Concat(recentCommissions)
            .OrderByDescending(a => a.CreatedUtc)
            .Take(10)
            .ToList();

        // Conversion rate
        var totalClicks = await clicksQuery.CountAsync(cancellationToken);
        var totalConversions = await clicksQuery.CountAsync(c => c.IsConverted, cancellationToken);
        var conversionRate = totalClicks > 0 ? (decimal)totalConversions / totalClicks * 100 : 0;

        return new PublisherDashboardDto
        {
            Profile = new PublisherProfileDto
            {
                Id = publisher.Id,
                PublisherCode = publisher.PublisherCode,
                CompanyName = publisher.CompanyName,
                WebsiteUrl = publisher.WebsiteUrl,
                Status = publisher.Status.ToString(),
                CreatedUtc = publisher.CreatedUtc
            },
            Stats = new DashboardStatsDto
            {
                TodayClicks = todayClicks,
                Last7DaysClicks = last7DaysClicks,
                TodayConversions = todayConversions,
                Last30DaysEarnings = last30DaysEarnings,
                AvailableBalance = availableBalance,
                PendingEarnings = pendingEarnings,
                ConversionRate = Math.Round(conversionRate, 2)
            },
            RecentActivities = recentActivities
        };
    }
}
