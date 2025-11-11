using Application.Abstractions.Persistence;
using Application.Features.Merchants.Queries;
using Domain.Entities.Affiliate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Merchants.Handlers
{
    public sealed class GetAllMerchantsHandler
        : IRequestHandler<GetAllMerchantsQuery, IEnumerable<Merchant>>
    {
        private readonly IAppDbContext _context;

        public GetAllMerchantsHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Merchant>> Handle(GetAllMerchantsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Merchants
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
