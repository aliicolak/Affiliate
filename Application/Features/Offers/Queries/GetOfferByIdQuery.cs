using Application.Abstractions;
using Domain.Entities.Affiliate;

namespace Application.Features.Offers.Queries
{
    public sealed record GetOfferByIdQuery(long Id) : IQuery<Offer?>;
}
