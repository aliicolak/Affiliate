using Application.Abstractions;

namespace Application.Features.Offers.Commands
{
    public sealed record CreateOfferCommand(
        long ProductId,
        long MerchantId,
        long? ProgramId,
        string AffiliateUrl,
        string? LandingUrl,
        decimal PriceAmount,
        int CurrencyId,
        bool InStock,
        decimal? ShippingFee
    ) : ICommand<long>;
}
