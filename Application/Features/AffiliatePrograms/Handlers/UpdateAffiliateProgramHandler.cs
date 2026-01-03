using Application.Abstractions.Persistence;
using Application.Features.AffiliatePrograms.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AffiliatePrograms.Handlers;

public sealed class UpdateAffiliateProgramHandler : IRequestHandler<UpdateAffiliateProgramCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateAffiliateProgramHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateAffiliateProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _context.AffiliatePrograms
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (program is null) return false;

        program.Name = request.Name;
        program.MerchantId = request.MerchantId;
        program.DefaultCurrencyId = request.DefaultCurrencyId;
        program.BaseCommissionPct = request.BaseCommissionPct;
        program.CookieDays = request.CookieDays;
        program.TrackingDomain = request.TrackingDomain;
        program.UpdatedUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
