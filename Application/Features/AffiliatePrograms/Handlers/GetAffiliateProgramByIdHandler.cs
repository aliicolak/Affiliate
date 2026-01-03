using Application.Abstractions.Persistence;
using Application.Features.AffiliatePrograms.DTOs;
using Application.Features.AffiliatePrograms.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AffiliatePrograms.Handlers;

public sealed class GetAffiliateProgramByIdHandler : IRequestHandler<GetAffiliateProgramByIdQuery, AffiliateProgramDto?>
{
    private readonly IAppDbContext _context;

    public GetAffiliateProgramByIdHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<AffiliateProgramDto?> Handle(GetAffiliateProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var program = await _context.AffiliatePrograms
            .AsNoTracking()
            .Include(p => p.Merchant)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (program is null) return null;

        var offerCount = await _context.Offers.CountAsync(o => o.ProgramId == program.Id, cancellationToken);

        return new AffiliateProgramDto
        {
            Id = program.Id,
            Name = program.Name,
            MerchantId = program.MerchantId,
            MerchantName = program.Merchant.Name,
            DefaultCurrencyId = program.DefaultCurrencyId,
            BaseCommissionPct = program.BaseCommissionPct,
            CookieDays = program.CookieDays,
            TrackingDomain = program.TrackingDomain,
            OfferCount = offerCount,
            CreatedUtc = program.CreatedUtc
        };
    }
}
