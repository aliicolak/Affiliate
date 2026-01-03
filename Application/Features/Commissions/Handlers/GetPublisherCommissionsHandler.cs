using Application.Abstractions.Persistence;
using Application.Features.Commissions.DTOs;
using Application.Features.Commissions.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commissions.Handlers;

/// <summary>
/// Publisher komisyon listesi handler'ı
/// </summary>
public sealed class GetPublisherCommissionsHandler 
    : IRequestHandler<GetPublisherCommissionsQuery, PagedCommissionsDto>
{
    private readonly IAppDbContext _context;

    public GetPublisherCommissionsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedCommissionsDto> Handle(
        GetPublisherCommissionsQuery request, 
        CancellationToken cancellationToken)
    {
        var query = _context.Commissions
            .AsNoTracking()
            .Include(c => c.ClickEvent)
                .ThenInclude(ce => ce.Offer)
                    .ThenInclude(o => o.Merchant)
            .Where(c => c.PublisherId == request.PublisherId);

        // Tarih filtresi
        if (request.StartDate.HasValue)
            query = query.Where(c => c.CreatedUtc >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(c => c.CreatedUtc <= request.EndDate.Value);

        // Durum filtresi
        if (!string.IsNullOrEmpty(request.Status) && 
            Enum.TryParse<CommissionStatus>(request.Status, true, out var status))
        {
            query = query.Where(c => c.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CommissionItemDto
            {
                Id = c.Id,
                MerchantName = c.ClickEvent.Offer.Merchant.Name,
                OfferName = c.ClickEvent.Offer.AffiliateUrl,
                SaleAmount = c.SaleAmount,
                CommissionAmount = c.CommissionAmount,
                CommissionRate = c.CommissionRate,
                Currency = c.Currency,
                Status = c.Status.ToString(),
                CreatedUtc = c.CreatedUtc,
                StatusChangedUtc = c.StatusChangedUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedCommissionsDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
