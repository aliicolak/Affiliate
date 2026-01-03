using Application.Abstractions.Persistence;
using Application.Features.Wishlists.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wishlists.Handlers;

public sealed class RemoveItemFromWishlistHandler : IRequestHandler<RemoveItemFromWishlistCommand, bool>
{
    private readonly IAppDbContext _context;

    public RemoveItemFromWishlistHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveItemFromWishlistCommand request, CancellationToken cancellationToken)
    {
        // Verify wishlist belongs to user
        var wishlist = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == request.WishlistId && w.UserId == request.UserId, cancellationToken);

        if (wishlist is null) return false;

        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(i => i.WishlistId == request.WishlistId && i.ProductId == request.ProductId, cancellationToken);

        if (item is null) return false;

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
