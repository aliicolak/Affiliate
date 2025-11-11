using Application.Abstractions;
using Domain.Entities.Affiliate;

namespace Application.Features.Merchants.Queries
{
    public sealed record GetAllMerchantsQuery() : IQuery<IEnumerable<Merchant>>;
}
