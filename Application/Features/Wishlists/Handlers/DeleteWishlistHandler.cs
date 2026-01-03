using Application.Abstractions.Persistence;
using Application.Features.Wishlists.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wishlists.Handlers;

public sealed class DeleteWishlistHandler : IRequestHandler<DeleteWishlistCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteWishlistHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteWishlistCommand request, CancellationToken cancellationToken)
    {
        var wishlist = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == request.UserId, cancellationToken);

        if (wishlist is null) return false;

        wishlist.DeletedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
