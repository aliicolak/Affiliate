using Application.Abstractions;
using Domain.Entities.Affiliate;
using MediatR;

namespace Application.Features.Offers.Commands
{
    public sealed record UpdateOfferCommand(
        long Id,
        long ProductId,
        long MerchantId,
        long? ProgramId,
        string AffiliateUrl,
        string? LandingUrl,
        decimal PriceAmount,
        int CurrencyId,
        bool InStock,
        decimal? ShippingFee
    ) : IRequest<bool>;
}
