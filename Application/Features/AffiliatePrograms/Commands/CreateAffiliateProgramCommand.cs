using Application.Abstractions;

namespace Application.Features.AffiliatePrograms.Commands;

public sealed record CreateAffiliateProgramCommand(
    string Name,
    long MerchantId,
    int DefaultCurrencyId,
    decimal BaseCommissionPct,
    int CookieDays,
    string? TrackingDomain
) : ICommand<long>;
