using Application.Abstractions.Persistence;
using Application.Features.Wishlists.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wishlists.Handlers;

public sealed class UpdateWishlistHandler : IRequestHandler<UpdateWishlistCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateWishlistHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateWishlistCommand request, CancellationToken cancellationToken)
    {
        var wishlist = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == request.UserId, cancellationToken);

        if (wishlist is null) return false;

        wishlist.Name = request.Name;
        wishlist.IsPublic = request.IsPublic;
        wishlist.UpdatedUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
