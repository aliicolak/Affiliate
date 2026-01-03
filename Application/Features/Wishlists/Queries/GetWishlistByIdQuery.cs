using Application.Abstractions;
using Application.Features.Wishlists.DTOs;

namespace Application.Features.Wishlists.Queries;

public sealed record GetWishlistByIdQuery(long Id, long UserId, string LanguageCode = "tr") : IQuery<WishlistDetailDto?>;
