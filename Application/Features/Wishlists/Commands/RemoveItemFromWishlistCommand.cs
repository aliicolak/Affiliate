using Application.Abstractions;

namespace Application.Features.Wishlists.Commands;

public sealed record RemoveItemFromWishlistCommand(long WishlistId, long ProductId, long UserId) : ICommand<bool>;
