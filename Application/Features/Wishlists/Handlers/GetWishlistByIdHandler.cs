using Application.Abstractions.Persistence;
using Application.Features.Wishlists.DTOs;
using Application.Features.Wishlists.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wishlists.Handlers;

public sealed class GetWishlistByIdHandler : IRequestHandler<GetWishlistByIdQuery, WishlistDetailDto?>
{
    private readonly IAppDbContext _context;

    public GetWishlistByIdHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<WishlistDetailDto?> Handle(GetWishlistByIdQuery request, CancellationToken cancellationToken)
    {
        var wishlist = await _context.Wishlists
            .AsNoTracking()
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == request.Id && 
                (w.UserId == request.UserId || w.IsPublic) && 
                w.DeletedUtc == null, cancellationToken);

        if (wishlist is null) return null;

        // Get product details for items
        var productIds = wishlist.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Translations)
            .Include(p => p.PrimaryImage)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var productLookup = products.ToDictionary(p => p.Id);

        return new WishlistDetailDto
        {
            Id = wishlist.Id,
            Name = wishlist.Name,
            IsPublic = wishlist.IsPublic,
            CreatedUtc = wishlist.CreatedUtc,
            Items = wishlist.Items.Select(i =>
            {
                var product = productLookup.GetValueOrDefault(i.ProductId);
                var translation = product?.Translations
                    .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
                    ?? product?.Translations.FirstOrDefault();

                return new WishlistItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = translation?.Name ?? "",
                    ProductSlug = product?.Slug ?? "",
                    ProductImageUrl = product?.PrimaryImage?.Url,
                    AddedUtc = i.AddedUtc
                };
            }).ToList()
        };
    }
}
