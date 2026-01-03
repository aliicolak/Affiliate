using Application.Abstractions.Persistence;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Raporlama ve analitik controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReportsController : ControllerBase
{
    private readonly IAppDbContext _context;

    public ReportsController(IAppDbContext context)
    {
        _context = context;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Platform genel istatistikleri (Admin)
    /// </summary>
    [HttpGet("platform-stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPlatformStats()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var thisMonth = new DateTime(now.Year, now.Month, 1);

        var stats = new
        {
            Merchants = new
            {
                Total = await _context.Merchants.CountAsync(),
                Active = await _context.Merchants.CountAsync(m => m.IsActive)
            },
            Publishers = new
            {
                Total = await _context.Publishers.CountAsync(),
                Active = await _context.Publishers.CountAsync(p => p.Status == PublisherStatus.Active)
            },
            Offers = new
            {
                Total = await _context.Offers.CountAsync(),
                Active = await _context.Offers.CountAsync(o => o.IsActive)
            },
            Clicks = new
            {
                Today = await _context.ClickEvents.CountAsync(c => c.CreatedUtc >= today),
                ThisMonth = await _context.ClickEvents.CountAsync(c => c.CreatedUtc >= thisMonth),
                Total = await _context.ClickEvents.CountAsync()
            },
            Conversions = new
            {
                Today = await _context.Conversions.CountAsync(c => c.CreatedUtc >= today),
                ThisMonth = await _context.Conversions.CountAsync(c => c.CreatedUtc >= thisMonth),
                Total = await _context.Conversions.CountAsync()
            },
            Commissions = new
            {
                TotalAmount = await _context.Commissions.SumAsync(c => c.CommissionAmount),
                PendingAmount = await _context.Commissions
                    .Where(c => c.Status == CommissionStatus.Pending)
                    .SumAsync(c => c.CommissionAmount),
                ApprovedAmount = await _context.Commissions
                    .Where(c => c.Status == CommissionStatus.Approved)
                    .SumAsync(c => c.CommissionAmount),
                PaidAmount = await _context.Commissions
                    .Where(c => c.Status == CommissionStatus.Paid)
                    .SumAsync(c => c.CommissionAmount)
            },
            VendorApplications = new
            {
                Pending = await _context.VendorApplications.CountAsync(a => a.Status == ApplicationStatus.Submitted),
                Approved = await _context.VendorApplications.CountAsync(a => a.Status == ApplicationStatus.Approved)
            }
        };

        return Ok(stats);
    }

    /// <summary>
    /// Merchant analitikleri (Admin veya Merchant sahibi)
    /// </summary>
    [HttpGet("merchant/{merchantId:long}")]
    public async Task<IActionResult> GetMerchantAnalytics(
        long merchantId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var offersQuery = _context.Offers.Where(o => o.MerchantId == merchantId);
        var offerIds = await offersQuery.Select(o => o.Id).ToListAsync();

        var clicksQuery = _context.ClickEvents
            .Where(c => offerIds.Contains(c.OfferId) && c.CreatedUtc >= start && c.CreatedUtc <= end);

        var totalClicks = await clicksQuery.CountAsync();
        var conversions = await clicksQuery.CountAsync(c => c.IsConverted);

        var dailyStats = await clicksQuery
            .GroupBy(c => c.CreatedUtc.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                Clicks = g.Count(),
                Conversions = g.Count(c => c.IsConverted)
            })
            .ToListAsync();

        var topOffers = await _context.ClickEvents
            .Where(c => offerIds.Contains(c.OfferId) && c.CreatedUtc >= start)
            .GroupBy(c => c.OfferId)
            .Select(g => new
            {
                OfferId = g.Key,
                Clicks = g.Count(),
                Conversions = g.Count(c => c.IsConverted)
            })
            .OrderByDescending(x => x.Clicks)
            .Take(10)
            .ToListAsync();

        return Ok(new
        {
            Period = new { Start = start, End = end },
            Summary = new
            {
                TotalClicks = totalClicks,
                TotalConversions = conversions,
                ConversionRate = totalClicks > 0 ? Math.Round((decimal)conversions / totalClicks * 100, 2) : 0
            },
            DailyStats = dailyStats,
            TopOffers = topOffers
        });
    }

    /// <summary>
    /// Publisher performans raporu
    /// </summary>
    [HttpGet("my-performance")]
    public async Task<IActionResult> GetMyPerformance(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userId = GetUserId();
        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (publisher is null)
            return NotFound("Publisher profile not found");

        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var clicksQuery = _context.ClickEvents
            .Where(c => c.PublisherId == publisher.Id && c.CreatedUtc >= start && c.CreatedUtc <= end);

        var totalClicks = await clicksQuery.CountAsync();
        var totalConversions = await clicksQuery.CountAsync(c => c.IsConverted);

        var commissionsQuery = _context.Commissions
            .Where(c => c.PublisherId == publisher.Id && c.CreatedUtc >= start && c.CreatedUtc <= end);

        var totalEarnings = await commissionsQuery
            .Where(c => c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid)
            .SumAsync(c => c.CommissionAmount);

        var dailyStats = await clicksQuery
            .GroupBy(c => c.CreatedUtc.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                Clicks = g.Count(),
                Conversions = g.Count(c => c.IsConverted)
            })
            .ToListAsync();

        var earningsByMerchant = await commissionsQuery
            .Include(c => c.ClickEvent)
                .ThenInclude(ce => ce.Offer)
                    .ThenInclude(o => o.Merchant)
            .Where(c => c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid)
            .GroupBy(c => c.ClickEvent.Offer.Merchant.Name)
            .Select(g => new
            {
                Merchant = g.Key,
                Earnings = g.Sum(c => c.CommissionAmount)
            })
            .OrderByDescending(x => x.Earnings)
            .Take(10)
            .ToListAsync();

        return Ok(new
        {
            Period = new { Start = start, End = end },
            Summary = new
            {
                TotalClicks = totalClicks,
                TotalConversions = totalConversions,
                ConversionRate = totalClicks > 0 ? Math.Round((decimal)totalConversions / totalClicks * 100, 2) : 0,
                TotalEarnings = totalEarnings
            },
            DailyStats = dailyStats,
            TopMerchants = earningsByMerchant
        });
    }
}
