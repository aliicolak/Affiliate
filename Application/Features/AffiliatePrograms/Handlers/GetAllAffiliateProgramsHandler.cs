using Application.Abstractions.Persistence;
using Application.Features.AffiliatePrograms.DTOs;
using Application.Features.AffiliatePrograms.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AffiliatePrograms.Handlers;

public sealed class GetAllAffiliateProgramsHandler : IRequestHandler<GetAllAffiliateProgramsQuery, List<AffiliateProgramDto>>
{
    private readonly IAppDbContext _context;

    public GetAllAffiliateProgramsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AffiliateProgramDto>> Handle(GetAllAffiliateProgramsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AffiliatePrograms
            .AsNoTracking()
            .Include(p => p.Merchant)
            .AsQueryable();

        if (request.MerchantId is not null)
            query = query.Where(p => p.MerchantId == request.MerchantId);

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new AffiliateProgramDto
            {
                Id = p.Id,
                Name = p.Name,
                MerchantId = p.MerchantId,
                MerchantName = p.Merchant.Name,
                DefaultCurrencyId = p.DefaultCurrencyId,
                BaseCommissionPct = p.BaseCommissionPct,
                CookieDays = p.CookieDays,
                TrackingDomain = p.TrackingDomain,
                OfferCount = _context.Offers.Count(o => o.ProgramId == p.Id),
                CreatedUtc = p.CreatedUtc
            })
            .ToListAsync(cancellationToken);
    }
}
