using Application.Abstractions.Persistence;
using Application.Features.Offers.Commands;
using Domain.Entities.Affiliate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Offers.Handlers
{
    public sealed class UpdateOfferHandler : IRequestHandler<UpdateOfferCommand, bool>
    {
        private readonly IAppDbContext _context;

        public UpdateOfferHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (offer is null)
                return false;

            // Güncelleme işlemi
            offer.PriceAmount = request.PriceAmount;
            offer.InStock = request.InStock;
            offer.AffiliateUrl = request.AffiliateUrl;
            offer.LandingUrl = request.LandingUrl;
            offer.UpdatedUtc = DateTime.UtcNow;

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
