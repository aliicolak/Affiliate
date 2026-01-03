using Application.Abstractions.Persistence;
using Application.Features.AffiliatePrograms.Commands;
using Domain.Entities.Affiliate;
using MediatR;

namespace Application.Features.AffiliatePrograms.Handlers;

public sealed class CreateAffiliateProgramHandler : IRequestHandler<CreateAffiliateProgramCommand, long>
{
    private readonly IAppDbContext _context;

    public CreateAffiliateProgramHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateAffiliateProgramCommand request, CancellationToken cancellationToken)
    {
        var program = new AffiliateProgram
        {
            Name = request.Name,
            MerchantId = request.MerchantId,
            DefaultCurrencyId = request.DefaultCurrencyId,
            BaseCommissionPct = request.BaseCommissionPct,
            CookieDays = request.CookieDays,
            TrackingDomain = request.TrackingDomain
        };

        _context.AffiliatePrograms.Add(program);
        await _context.SaveChangesAsync(cancellationToken);

        return program.Id;
    }
}
