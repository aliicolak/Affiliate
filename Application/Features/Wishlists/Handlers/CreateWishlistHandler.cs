using Application.Abstractions.Persistence;
using Application.Features.Wishlists.Commands;
using Domain.Entities.UserContent;
using MediatR;

namespace Application.Features.Wishlists.Handlers;

public sealed class CreateWishlistHandler : IRequestHandler<CreateWishlistCommand, long>
{
    private readonly IAppDbContext _context;

    public CreateWishlistHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
    {
        var wishlist = new Wishlist
        {
            UserId = request.UserId,
            Name = request.Name,
            IsPublic = request.IsPublic
        };

        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync(cancellationToken);

        return wishlist.Id;
    }
}
