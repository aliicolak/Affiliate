using Application.Abstractions;
using Application.Features.Wishlists.DTOs;

namespace Application.Features.Wishlists.Queries;

public sealed record GetUserWishlistsQuery(long UserId) : IQuery<List<WishlistDto>>;
