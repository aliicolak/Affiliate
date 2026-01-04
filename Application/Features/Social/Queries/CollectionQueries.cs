using MediatR;
using Application.Features.Social;

namespace Application.Features.Social.Queries;

public record GetMyCollectionsQuery : IRequest<List<CollectionDto>>;

public record GetUserCollectionsQuery(long UserId) : IRequest<List<CollectionDto>>;

public record GetCollectionDetailQuery(long Id) : IRequest<CollectionDetailDto>;

public record SearchCollectionsQuery(string? SearchTerm) : IRequest<List<CollectionDto>>;
