using Application.Abstractions.Persistence;
using Application.Features.Blog.Commands;
using Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Social.Commands;

public class CollectionCommandHandlers : 
    IRequestHandler<CreateCollectionCommand, long>,
    IRequestHandler<DeleteCollectionCommand, Unit>,
    IRequestHandler<AddItemToCollectionCommand, long>,
    IRequestHandler<RemoveItemFromCollectionCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CollectionCommandHandlers(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<long> Handle(CreateCollectionCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var collection = new Collection
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic,
            IsDefault = false
        };
        _db.Collections.Add(collection);
        await _db.SaveChangesAsync(ct);
        return collection.Id;
    }

    public async Task<Unit> Handle(DeleteCollectionCommand request, CancellationToken ct)
    {
         var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
         var collection = await _db.Collections.FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userId, ct);
         if (collection == null) throw new KeyNotFoundException("Collection not found or unauthorized");

         _db.Collections.Remove(collection);
         await _db.SaveChangesAsync(ct);
         return Unit.Value;
    }

    public async Task<long> Handle(AddItemToCollectionCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var collection = await _db.Collections.FirstOrDefaultAsync(c => c.Id == request.CollectionId && c.UserId == userId, ct);
        if (collection == null) throw new KeyNotFoundException("Collection not found or unauthorized");

        // Check if item already exists
        var exists = await _db.CollectionItems.AnyAsync(i => i.CollectionId == request.CollectionId && i.EntityType == request.EntityType && i.EntityId == request.EntityId, ct);
        if (exists)
        {
            // Or return existing ID? For now, throw or return existing
             var existing = await _db.CollectionItems.FirstAsync(i => i.CollectionId == request.CollectionId && i.EntityType == request.EntityType && i.EntityId == request.EntityId, ct);
             return existing.Id;
        }

        var item = new CollectionItem
        {
            CollectionId = request.CollectionId,
            EntityType = request.EntityType,
            EntityId = request.EntityId
        };
        _db.CollectionItems.Add(item);
        await _db.SaveChangesAsync(ct);
        return item.Id;
    }

    public async Task<Unit> Handle(RemoveItemFromCollectionCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var item = await _db.CollectionItems
            .Include(i => i.Collection)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, ct);

        if (item == null) throw new KeyNotFoundException("Item not found");
        if (item.Collection.UserId != userId) throw new UnauthorizedAccessException();

        _db.CollectionItems.Remove(item);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
