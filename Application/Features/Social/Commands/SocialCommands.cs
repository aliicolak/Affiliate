using MediatR;

namespace Application.Features.Social.Commands;

// ===== Like Commands =====

/// <summary>
/// Toggle like on an entity (blog post, share, comment)
/// </summary>
public record ToggleLikeCommand(
    string EntityType, // BlogPost, ProductShare, BlogComment, ShareComment
    long EntityId
) : IRequest<LikeResult>;

public record LikeResult(bool IsLiked, int LikeCount);

// ===== ProductShare Commands =====

public record CreateShareCommand(
    long OfferId,
    string Title,
    string? Description,
    string ShareType, // Recommendation, Review, Showcase, Comparison
    int? Rating,
    string? Pros,
    string? Cons,
    string? ImageUrl
) : IRequest<ProductShareDto>;

public record UpdateShareCommand(
    long Id,
    string Title,
    string? Description,
    int? Rating,
    string? Pros,
    string? Cons,
    string? ImageUrl
) : IRequest<ProductShareDto?>;

public record DeleteShareCommand(long Id) : IRequest<bool>;

// ===== ShareComment Commands =====

public record CreateShareCommentCommand(
    long ShareId,
    string Content,
    long? ParentCommentId = null
) : IRequest<ShareCommentDto>;

public record DeleteShareCommentCommand(long Id) : IRequest<bool>;

// ===== Follow Commands =====

/// <summary>
/// Toggle follow/unfollow a user
/// </summary>
public record ToggleFollowCommand(long UserId) : IRequest<FollowResult>;

public record FollowResult(bool IsFollowing, int FollowerCount);

// ===== User Profile Commands =====

public record UpdateUserProfileCommand(
    string DisplayName,
    string? Bio,
    string? AvatarUrl
) : IRequest<bool>;
