using Application.Abstractions.Persistence;
using Application.Features.Merchants.Queries;
using Domain.Entities.Affiliate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Merchants.Handlers
{
    public sealed class GetMerchantByIdHandler
        : IRequestHandler<GetMerchantByIdQuery, Merchant?>
    {
        private readonly IAppDbContext _context;

        public GetMerchantByIdHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Merchant?> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Merchants
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        }
    }
}
