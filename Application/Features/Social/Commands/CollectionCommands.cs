using MediatR;

namespace Application.Features.Social.Commands;

public record CreateCollectionCommand(string Name, string? Description, bool IsPublic) : IRequest<long>;

public record DeleteCollectionCommand(long Id) : IRequest<Unit>;

public record AddItemToCollectionCommand(long CollectionId, string EntityType, long EntityId) : IRequest<long>;

public record RemoveItemFromCollectionCommand(long ItemId) : IRequest<Unit>;
