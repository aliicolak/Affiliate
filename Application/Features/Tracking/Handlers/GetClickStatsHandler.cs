using Application.Abstractions.Persistence;
using Application.Features.Tracking.DTOs;
using Application.Features.Tracking.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.Handlers;

/// <summary>
/// Tıklama istatistikleri handler'ı
/// </summary>
public sealed class GetClickStatsHandler : IRequestHandler<GetClickStatsQuery, ClickStatsDto>
{
    private readonly IAppDbContext _context;

    public GetClickStatsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClickStatsDto> Handle(GetClickStatsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ClickEvents.AsNoTracking().AsQueryable();

        // Filtreler
        if (request.PublisherId.HasValue)
            query = query.Where(c => c.PublisherId == request.PublisherId);

        if (request.OfferId.HasValue)
            query = query.Where(c => c.OfferId == request.OfferId);

        if (request.MerchantId.HasValue)
            query = query.Where(c => c.Offer.MerchantId == request.MerchantId);

        if (request.StartDate.HasValue)
            query = query.Where(c => c.CreatedUtc >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(c => c.CreatedUtc <= request.EndDate.Value);

        // Toplam istatistikler
        var totalClicks = await query.CountAsync(cancellationToken);
        var uniqueClicks = await query.Select(c => c.IpAddress).Distinct().CountAsync(cancellationToken);
        var conversions = await query.CountAsync(c => c.IsConverted, cancellationToken);

        // Ülke bazlı dağılım
        var byCountry = await query
            .Where(c => c.CountryCode != null)
            .GroupBy(c => c.CountryCode!)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Country, x => x.Count, cancellationToken);

        // Cihaz bazlı dağılım
        var byDevice = await query
            .GroupBy(c => c.DeviceType)
            .Select(g => new { Device = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Device, x => x.Count, cancellationToken);

        // Günlük istatistikler
        var dailyStats = await query
            .GroupBy(c => c.CreatedUtc.Date)
            .OrderBy(g => g.Key)
            .Select(g => new ClickStatItemDto
            {
                Date = g.Key,
                Clicks = g.Count(),
                UniqueClicks = g.Select(c => c.IpAddress).Distinct().Count(),
                Conversions = g.Count(c => c.IsConverted)
            })
            .ToListAsync(cancellationToken);

        return new ClickStatsDto
        {
            TotalClicks = totalClicks,
            UniqueClicks = uniqueClicks,
            Conversions = conversions,
            Items = dailyStats,
            ByCountry = byCountry,
            ByDevice = byDevice
        };
    }
}
