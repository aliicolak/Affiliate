using Application.Abstractions.Persistence;
using Application.Features.Wishlists.DTOs;
using Application.Features.Wishlists.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wishlists.Handlers;

public sealed class GetUserWishlistsHandler : IRequestHandler<GetUserWishlistsQuery, List<WishlistDto>>
{
    private readonly IAppDbContext _context;

    public GetUserWishlistsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishlistDto>> Handle(GetUserWishlistsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Wishlists
            .AsNoTracking()
            .Where(w => w.UserId == request.UserId && w.DeletedUtc == null)
            .Include(w => w.Items)
            .OrderByDescending(w => w.CreatedUtc)
            .Select(w => new WishlistDto
            {
                Id = w.Id,
                Name = w.Name,
                IsPublic = w.IsPublic,
                ItemCount = w.Items.Count,
                CreatedUtc = w.CreatedUtc
            })
            .ToListAsync(cancellationToken);
    }
}
