using Application.Abstractions.Persistence;
using Application.Features.Tracking.DTOs;
using Application.Features.Tracking.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.Handlers;

/// <summary>
/// Publisher tıklama listesi handler'ı
/// </summary>
public sealed class GetPublisherClicksHandler : IRequestHandler<GetPublisherClicksQuery, PagedClicksDto>
{
    private readonly IAppDbContext _context;

    public GetPublisherClicksHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedClicksDto> Handle(GetPublisherClicksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ClickEvents
            .AsNoTracking()
            .Include(c => c.Offer)
                .ThenInclude(o => o.Merchant)
            .Where(c => c.PublisherId == request.PublisherId);

        if (request.StartDate.HasValue)
            query = query.Where(c => c.CreatedUtc >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(c => c.CreatedUtc <= request.EndDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ClickItemDto
            {
                Id = c.Id,
                OfferId = c.OfferId,
                OfferName = c.Offer.AffiliateUrl,
                MerchantName = c.Offer.Merchant.Name,
                TrackingCode = c.TrackingCode,
                CountryCode = c.CountryCode,
                DeviceType = c.DeviceType.ToString(),
                IsConverted = c.IsConverted,
                CreatedUtc = c.CreatedUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedClicksDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
