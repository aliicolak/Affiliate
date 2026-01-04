using MediatR;

namespace Application.Features.Social.Queries;

// ===== ProductShare Queries =====

public record GetShareQuery(long Id) : IRequest<ProductShareDto?>;

public record GetSharesQuery(
    long? UserId = null,
    long? OfferId = null,
    string? ShareType = null,
    string SortBy = "newest", // newest, popular, rating
    int Page = 1,
    int PageSize = 10
) : IRequest<PaginatedSharesResult>;

public record GetFeedQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedSharesResult>;

// ===== ShareComment Queries =====

public record GetShareCommentsQuery(
    long ShareId,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<ShareCommentDto>>;

// ===== Follow Queries =====

public record GetFollowersQuery(
    long UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedFollowersResult>;

public record GetFollowingQuery(
    long UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedFollowersResult>;

// ===== Profile Queries =====

public record GetUserProfileQuery(long UserId) : IRequest<UserProfileDto?>;

public record IsFollowingQuery(long UserId) : IRequest<bool>;
