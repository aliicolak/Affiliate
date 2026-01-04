using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Features.Blog.Commands;
using Domain.Entities.Social;
using Domain.Enums;

namespace Application.Features.Social.Commands;

// ===== Like Handlers =====

public class ToggleLikeHandler : IRequestHandler<ToggleLikeCommand, LikeResult>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public ToggleLikeHandler(IAppDbContext db, ICurrentUserService currentUser, INotificationService notificationService)
    {
        _db = db;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<LikeResult> Handle(ToggleLikeCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

        var existingLike = await _db.Likes
            .FirstOrDefaultAsync(l => l.UserId == userId && 
                                      l.EntityType == request.EntityType && 
                                      l.EntityId == request.EntityId, ct);

        bool isLiked;
        if (existingLike != null)
        {
            // Unlike
            _db.Likes.Remove(existingLike);
            isLiked = false;
        }
        else
        {
            // Like
            _db.Likes.Add(new Like
            {
                UserId = userId,
                EntityType = request.EntityType,
                EntityId = request.EntityId
            });
            isLiked = true;
        }

        // Update like count on the entity
        var ownerId = await UpdateEntityAndGetOwner(request.EntityType, request.EntityId, isLiked ? 1 : -1, ct);
        await _db.SaveChangesAsync(ct);

        if (isLiked && ownerId.HasValue && ownerId.Value != userId)
        {
            await _notificationService.SendAsync(
                ownerId.Value,
                NotificationType.SocialLike,
                "New Like",
                $"Someone liked your {request.EntityType}",
                actionUrl: request.EntityType == "ProductShare" ? $"/social/share/{request.EntityId}" : null,
                relatedEntityType: request.EntityType,
                relatedEntityId: request.EntityId,
                cancellationToken: ct);
        }

        var likeCount = await _db.Likes.CountAsync(l => 
            l.EntityType == request.EntityType && l.EntityId == request.EntityId, ct);

        return new LikeResult(isLiked, likeCount);
    }

    private async Task<long?> UpdateEntityAndGetOwner(string entityType, long entityId, int delta, CancellationToken ct)
    {
        long? ownerId = null;
        switch (entityType)
        {
            case "BlogPost":
                var post = await _db.BlogPosts.FindAsync(new object[] { entityId }, ct);
                if (post != null) { post.LikeCount = Math.Max(0, post.LikeCount + delta); ownerId = post.AuthorId; }
                break;
            case "ProductShare":
                var share = await _db.ProductShares.FindAsync(new object[] { entityId }, ct);
                if (share != null) { share.LikeCount = Math.Max(0, share.LikeCount + delta); ownerId = share.UserId; }
                break;
            case "BlogComment":
                var blogComment = await _db.BlogComments.FindAsync(new object[] { entityId }, ct);
                if (blogComment != null) { blogComment.LikeCount = Math.Max(0, blogComment.LikeCount + delta); ownerId = blogComment.UserId; }
                break;
            case "ShareComment":
                var shareComment = await _db.ShareComments.FindAsync(new object[] { entityId }, ct);
                if (shareComment != null) { shareComment.LikeCount = Math.Max(0, shareComment.LikeCount + delta); ownerId = shareComment.UserId; }
                break;
        }
        return ownerId;
    }
}

// ===== ProductShare Handlers =====

public class CreateShareHandler : IRequestHandler<CreateShareCommand, ProductShareDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateShareHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProductShareDto> Handle(CreateShareCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

        var share = new ProductShare
        {
            UserId = userId,
            OfferId = request.OfferId,
            Title = request.Title,
            Description = request.Description,
            ShareType = Enum.Parse<ShareType>(request.ShareType),
            Rating = request.Rating,
            Pros = request.Pros,
            Cons = request.Cons,
            ImageUrl = request.ImageUrl
        };

        _db.ProductShares.Add(share);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync(new object[] { userId }, ct);
        var offer = share.OfferId.HasValue 
            ? await _db.Offers
                .Include(o => o.Product)
                    .ThenInclude(p => p!.Translations)
                .Include(o => o.Product)
                    .ThenInclude(p => p!.PrimaryImage)
                .FirstOrDefaultAsync(o => o.Id == share.OfferId.Value, ct) 
            : null;

        return new ProductShareDto(
            share.Id,
            share.UserId,
            user?.DisplayName ?? "Unknown",
            user?.AvatarUrl,
            share.OfferId,
            offer?.Product?.Translations.FirstOrDefault()?.Name ?? "Unknown Product",
            offer?.Product?.PrimaryImage?.Url,
            share.Title,
            share.Description,
            share.ShareType.ToString(),
            share.Rating,
            share.Pros,
            share.Cons,
            share.ImageUrl,
            share.ViewCount,
            share.LikeCount,
            share.CommentCount,
            false,
            share.CreatedUtc
        );
    }
}

public class DeleteShareHandler : IRequestHandler<DeleteShareCommand, bool>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteShareHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteShareCommand request, CancellationToken ct)
    {
        var share = await _db.ProductShares.FindAsync(new object[] { request.Id }, ct);
        if (share == null) return false;

        var userId = _currentUser.UserId;
        if (share.UserId != userId && !_currentUser.IsInRole("Admin"))
            throw new UnauthorizedAccessException();

        _db.ProductShares.Remove(share);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

// ===== ShareComment Handlers =====

public class CreateShareCommentHandler : IRequestHandler<CreateShareCommentCommand, ShareCommentDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public CreateShareCommentHandler(IAppDbContext db, ICurrentUserService currentUser, INotificationService notificationService)
    {
        _db = db;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<ShareCommentDto> Handle(CreateShareCommentCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

        var share = await _db.ProductShares.FindAsync(new object[] { request.ShareId }, ct);
        if (share == null)
            throw new ArgumentException("Share not found");

        var comment = new ShareComment
        {
            ShareId = request.ShareId,
            UserId = userId,
            ParentCommentId = request.ParentCommentId,
            Content = request.Content
        };

        _db.ShareComments.Add(comment);
        share.CommentCount++;
        await _db.SaveChangesAsync(ct);

        // Notify Share Owner
        if (share.UserId != userId)
        {
            await _notificationService.SendAsync(
                share.UserId,
                NotificationType.SocialComment,
                "New Comment",
                "Someone commented on your share",
                actionUrl: $"/social/share/{share.Id}",
                relatedEntityType: "ProductShare",
                relatedEntityId: share.Id,
                cancellationToken: ct
            );
        }
        
        // Notify Parent Comment Owner
        if (request.ParentCommentId.HasValue)
        {
             // We need to fetch parent to know author. Not fetched yet.
             // EF Core tracks it if attached, but logic above used ID.
             // We can find local or fetch.
             var parent = await _db.ShareComments.FindAsync(new object[] { request.ParentCommentId.Value }, ct);
             if (parent != null && parent.UserId != userId && parent.UserId != share.UserId)
             {
                 await _notificationService.SendAsync(
                    parent.UserId,
                    NotificationType.SocialComment,
                    "New Reply",
                    "Someone replied to your comment",
                    actionUrl: $"/social/share/{share.Id}",
                    relatedEntityType: "ShareComment",
                    relatedEntityId: parent.Id,
                    cancellationToken: ct
                );
             }
        }

        var user = await _db.Users.FindAsync(new object[] { userId }, ct);

        return new ShareCommentDto(
            comment.Id,
            comment.ShareId,
            comment.UserId,
            user?.DisplayName ?? "Unknown",
            user?.AvatarUrl,
            comment.ParentCommentId,
            comment.Content,
            comment.LikeCount,
            comment.CreatedUtc,
            null
        );
    }
}

// ===== Follow Handlers =====

public class ToggleFollowHandler : IRequestHandler<ToggleFollowCommand, FollowResult>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public ToggleFollowHandler(IAppDbContext db, ICurrentUserService currentUser, INotificationService notificationService)
    {
        _db = db;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<FollowResult> Handle(ToggleFollowCommand request, CancellationToken ct)
    {
        var followerId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        
        if (followerId == request.UserId)
            throw new ArgumentException("Cannot follow yourself");

        var existingFollow = await _db.UserFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == request.UserId, ct);

        bool isFollowing;
        if (existingFollow != null)
        {
            // Unfollow
            _db.UserFollows.Remove(existingFollow);
            isFollowing = false;
        }
        else
        {
            // Follow
            _db.UserFollows.Add(new UserFollow
            {
                FollowerId = followerId,
                FollowingId = request.UserId
            });
            isFollowing = true;
        }

        await _db.SaveChangesAsync(ct);

        if (isFollowing)
        {
            await _notificationService.SendAsync(
                request.UserId, 
                NotificationType.SocialFollow,
                "New Follower",
                "Someone started following you",
                actionUrl: $"/profile/{followerId}",
                relatedEntityType: "User",
                relatedEntityId: followerId,
                cancellationToken: ct
            );
        }

        var followerCount = await _db.UserFollows.CountAsync(f => f.FollowingId == request.UserId, ct);

        return new FollowResult(isFollowing, followerCount);
    }
}

// ===== User Profile Handlers =====

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserProfileHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        // Since IdentityUser uses string Id in some contexts but here ApplicationUser : IdentityUser<long>
        // We assume DbSet<ApplicationUser> Users works with expression.
        // However, IAppDbContext interface definition of Users might require explicit cast if generic.
        // Usually it's DbSet<ApplicationUser> Users.
        
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null) return false;

        user.DisplayName = request.DisplayName;
        user.Bio = request.Bio;
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
