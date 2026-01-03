using Application.Abstractions.Persistence;
using Application.Features.Wishlists.Commands;
using Domain.Entities.UserContent;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wishlists.Handlers;

public sealed class AddItemToWishlistHandler : IRequestHandler<AddItemToWishlistCommand, bool>
{
    private readonly IAppDbContext _context;

    public AddItemToWishlistHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AddItemToWishlistCommand request, CancellationToken cancellationToken)
    {
        // Verify wishlist belongs to user
        var wishlist = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == request.WishlistId && w.UserId == request.UserId, cancellationToken);

        if (wishlist is null) return false;

        // Check if item already exists
        var exists = await _context.WishlistItems
            .AnyAsync(i => i.WishlistId == request.WishlistId && i.ProductId == request.ProductId, cancellationToken);

        if (exists) return true; // Already added

        var item = new WishlistItem
        {
            WishlistId = request.WishlistId,
            ProductId = request.ProductId,
            AddedUtc = DateTime.UtcNow
        };

        _context.WishlistItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
