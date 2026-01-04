using Application.Abstractions.Persistence;
using Application.Features.Blog.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Social;
using Domain.Entities.Affiliate;

namespace Application.Features.Social.Queries;

public class CollectionQueryHandlers : 
    IRequestHandler<GetMyCollectionsQuery, List<CollectionDto>>,
    IRequestHandler<GetUserCollectionsQuery, List<CollectionDto>>,
    IRequestHandler<GetCollectionDetailQuery, CollectionDetailDto>,
    IRequestHandler<SearchCollectionsQuery, List<CollectionDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CollectionQueryHandlers(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<CollectionDto>> Handle(GetMyCollectionsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        return await _db.Collections
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedUtc)
            .Select(c => new CollectionDto(
                c.Id,
                c.Name,
                c.Description,
                c.IsPublic,
                c.IsDefault,
                c.Items.Count,
                c.CreatedUtc
            ))
            .ToListAsync(ct);
    }

    public async Task<List<CollectionDto>> Handle(GetUserCollectionsQuery request, CancellationToken ct)
    {
        return await _db.Collections
            .Where(c => c.UserId == request.UserId && c.IsPublic)
            .OrderByDescending(c => c.CreatedUtc)
            .Select(c => new CollectionDto(
                c.Id,
                c.Name,
                c.Description,
                c.IsPublic,
                c.IsDefault,
                c.Items.Count,
                c.CreatedUtc
            ))
            .ToListAsync(ct);
    }

    public async Task<CollectionDetailDto> Handle(GetCollectionDetailQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var collection = await _db.Collections
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (collection == null) throw new KeyNotFoundException("Collection not found");

        if (!collection.IsPublic && collection.UserId != userId)
        {
            throw new UnauthorizedAccessException("This collection is private");
        }

        var shareIds = collection.Items.Where(i => i.EntityType == "ProductShare").Select(i => i.EntityId).ToList();
        var offerIds = collection.Items.Where(i => i.EntityType == "Offer").Select(i => i.EntityId).ToList();

        var shares = shareIds.Any() 
            ? await _db.ProductShares.Include(s => s.Offer).Where(s => shareIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, ct)
            : new Dictionary<long, ProductShare>();
            
        var offers = offerIds.Any()
            ? await _db.Offers
                .Include(o => o.Product).ThenInclude(p => p.Translations)
                .Where(o => offerIds.Contains(o.Id)).ToDictionaryAsync(o => o.Id, ct)
            : new Dictionary<long, Offer>();

        var items = collection.Items.Select(i => {
            object? data = null;
            if (i.EntityType == "ProductShare" && shares.TryGetValue(i.EntityId, out var share))
            {
                data = new { share.Id, share.Title, share.ImageUrl, share.LikeCount, share.CommentCount, share.ShareType, Price = share.Offer?.PriceAmount };
            }
            else if (i.EntityType == "Offer" && offers.TryGetValue(i.EntityId, out var offer))
            {
                var title = offer.Product?.Translations.FirstOrDefault()?.Name ?? "Product";
                data = new { offer.Id, Title = title, Price = offer.PriceAmount, ImageUrl = "", Url = offer.AffiliateUrl, offer.MerchantId };
            }

            return new CollectionItemDto(i.Id, i.EntityType, i.EntityId, data, i.CreatedUtc);
        }).OrderByDescending(i => i.AddedUtc).ToList();

        return new CollectionDetailDto(
            collection.Id, 
            collection.Name, 
            collection.Description, 
            collection.IsPublic, 
            collection.IsDefault, 
            items
        );
    }

    public async Task<List<CollectionDto>> Handle(SearchCollectionsQuery request, CancellationToken ct)
    {
        var query = _db.Collections.Where(c => c.IsPublic);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
             query = query.Where(c => c.Name.Contains(request.SearchTerm) || (c.Description != null && c.Description.Contains(request.SearchTerm)));
        }

        return await query
            .OrderByDescending(c => c.CreatedUtc)
            .Take(50)
            .Select(c => new CollectionDto(
                c.Id,
                c.Name,
                c.Description,
                c.IsPublic,
                c.IsDefault,
                c.Items.Count,
                c.CreatedUtc
            ))
            .ToListAsync(ct);
    }
}
