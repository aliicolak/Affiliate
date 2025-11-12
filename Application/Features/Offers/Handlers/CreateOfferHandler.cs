using Application.Abstractions.Persistence;
using Application.Features.Offers.Commands;
using Domain.Entities.Affiliate;
using MediatR;

namespace Application.Features.Offers.Handlers
{
    public sealed class CreateOfferHandler : IRequestHandler<CreateOfferCommand, long>
    {
        private readonly IAppDbContext _context;

        public CreateOfferHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = new Offer
            {
                ProductId = request.ProductId,
                MerchantId = request.MerchantId,
                ProgramId = request.ProgramId,
                AffiliateUrl = request.AffiliateUrl,
                LandingUrl = request.LandingUrl,
                PriceAmount = request.PriceAmount,
                CurrencyId = request.CurrencyId,
                InStock = request.InStock,
                ShippingFee = request.ShippingFee,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync(cancellationToken);
            return offer.Id;
        }
    }
}
