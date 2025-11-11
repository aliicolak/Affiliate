using Application.Abstractions.Persistence;
using Application.Features.Merchants.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Merchants.Handlers
{
    public sealed class DeleteMerchantHandler
        : IRequestHandler<DeleteMerchantCommand, bool>
    {
        private readonly IAppDbContext _context;

        public DeleteMerchantHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (merchant is null)
                return false;

            _context.Merchants.Remove(merchant);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
