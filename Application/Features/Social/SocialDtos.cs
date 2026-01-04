namespace Application.Features.Social;

// ===== DTOs =====

public record LikeDto(
    long Id,
    long UserId,
    string UserName,
    string EntityType,
    long EntityId,
    DateTime CreatedUtc
);

public record ProductShareDto(
    long Id,
    long UserId,
    string UserName,
    string? UserAvatarUrl,
    long? OfferId,
    string? OfferName,
    string? OfferImageUrl,
    string Title,
    string? Description,
    string ShareType,
    int? Rating,
    string? Pros,
    string? Cons,
    string? ImageUrl,
    int ViewCount,
    int LikeCount,
    int CommentCount,
    bool IsLikedByCurrentUser,
    DateTime CreatedUtc
);

public record ProductShareListDto(
    long Id,
    long UserId,
    string UserName,
    string? UserAvatarUrl,
    long? OfferId,
    string? OfferName,
    string? OfferImageUrl,
    string Title,
    string ShareType,
    int? Rating,
    int ViewCount,
    int LikeCount,
    int CommentCount,
    DateTime CreatedUtc
);

public record ShareCommentDto(
    long Id,
    long ShareId,
    long UserId,
    string UserName,
    string? UserAvatarUrl,
    long? ParentCommentId,
    string Content,
    int LikeCount,
    DateTime CreatedUtc,
    List<ShareCommentDto>? Replies
);

public record UserFollowDto(
    long Id,
    long FollowerId,
    string FollowerName,
    string? FollowerAvatarUrl,
    long FollowingId,
    string FollowingName,
    string? FollowingAvatarUrl,
    DateTime CreatedUtc
);

public record UserProfileDto(
    long Id,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    int FollowerCount,
    int FollowingCount,
    int PostCount,
    int ShareCount,
    bool IsFollowedByCurrentUser,
    DateTime JoinedUtc
);

// ===== Paginated Results =====

public record PaginatedSharesResult(
    List<ProductShareListDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record PaginatedFollowersResult(
    List<UserFollowDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
