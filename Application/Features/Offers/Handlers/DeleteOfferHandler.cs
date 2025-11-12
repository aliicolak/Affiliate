using Application.Abstractions.Persistence;
using Application.Features.Offers.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Offers.Handlers
{
    public sealed class DeleteOfferHandler : IRequestHandler<DeleteOfferCommand, bool>
    {
        private readonly IAppDbContext _context;

        public DeleteOfferHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (offer is null)
                return false;

            offer.DeletedUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
