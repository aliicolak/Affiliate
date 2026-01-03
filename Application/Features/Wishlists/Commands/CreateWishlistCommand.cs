using Application.Abstractions;

namespace Application.Features.Wishlists.Commands;

public sealed record CreateWishlistCommand(
    long UserId,
    string Name,
    bool IsPublic = false
) : ICommand<long>;
