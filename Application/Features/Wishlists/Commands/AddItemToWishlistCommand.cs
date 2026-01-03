using Application.Abstractions;

namespace Application.Features.Wishlists.Commands;

public sealed record AddItemToWishlistCommand(long WishlistId, long ProductId, long UserId) : ICommand<bool>;
