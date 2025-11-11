using Application.Abstractions.Persistence;
using Application.Features.Merchants.Commands;
using Domain.Entities.Affiliate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Merchants.Handlers
{
    public sealed class UpdateMerchantHandler
        : IRequestHandler<UpdateMerchantCommand, Merchant?>
    {
        private readonly IAppDbContext _context;

        public UpdateMerchantHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Merchant?> Handle(UpdateMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (merchant is null)
                return null;

            merchant.Name = request.Name;
            merchant.Website = request.Website;
            merchant.IsActive = request.IsActive;
            merchant.UpdatedUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return merchant;
        }
    }
}
