using Application.Abstractions;
using Application.Common.Responses;
using Application.Features.Offers.DTOs;

namespace Application.Features.Offers.Queries
{
    public sealed record GetAllOffersQuery(GetAllOffersRequestDto RequestDto)
        : IQuery<PagedResponse<OfferListResponseDto>>;
}
