using Application.Abstractions.Persistence;
using Application.Features.Offers.Queries;
using Domain.Entities.Affiliate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Offers.Handlers
{
    public sealed class GetOfferByIdHandler : IRequestHandler<GetOfferByIdQuery, Offer?>
    {
        private readonly IAppDbContext _context;

        public GetOfferByIdHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Offer?> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Offers
                .Include(o => o.Merchant)
                .Include(o => o.Program)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        }
    }
}
