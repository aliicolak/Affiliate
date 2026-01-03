using Application.Abstractions;

namespace Application.Features.Wishlists.Commands;

public sealed record DeleteWishlistCommand(long Id, long UserId) : ICommand<bool>;
