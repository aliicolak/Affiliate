using Application.Abstractions;

namespace Application.Features.Wishlists.Commands;

public sealed record UpdateWishlistCommand(
    long Id,
    long UserId,
    string Name,
    bool IsPublic
) : ICommand<bool>;
