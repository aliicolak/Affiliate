using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Persistence;
using Application.Features.Blog.Commands;

namespace Application.Features.Social.Queries;

public class GetShareHandler : IRequestHandler<GetShareQuery, ProductShareDto?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetShareHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProductShareDto?> Handle(GetShareQuery request, CancellationToken ct)
    {
        var share = await _db.ProductShares
            .Include(s => s.User)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Product)
                    .ThenInclude(p => p!.Translations)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Product)
                    .ThenInclude(p => p!.PrimaryImage)
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (share == null) return null;

        // Increment view count
        share.ViewCount++;
        await _db.SaveChangesAsync(ct);

        var isLiked = _currentUser.UserId.HasValue && await _db.Likes
            .AnyAsync(l => l.UserId == _currentUser.UserId && l.EntityType == "ProductShare" && l.EntityId == share.Id, ct);

        return new ProductShareDto(
            share.Id,
            share.UserId,
            share.User?.DisplayName ?? "Unknown",
            share.User?.AvatarUrl,
            share.OfferId,
            share.Offer?.Product?.Translations.FirstOrDefault()?.Name ?? "Unknown Product",
            share.Offer?.Product?.PrimaryImage?.Url,
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
            isLiked,
            share.CreatedUtc
        );
    }
}

public class GetSharesHandler : IRequestHandler<GetSharesQuery, PaginatedSharesResult>
{
    private readonly IAppDbContext _db;

    public GetSharesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedSharesResult> Handle(GetSharesQuery request, CancellationToken ct)
    {
        var query = _db.ProductShares
            .Include(s => s.User)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Product)
                    .ThenInclude(p => p!.Translations)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Product)
                    .ThenInclude(p => p!.PrimaryImage)
            .AsQueryable();

        if (request.UserId.HasValue)
            query = query.Where(s => s.UserId == request.UserId);

        if (request.OfferId.HasValue)
            query = query.Where(s => s.OfferId == request.OfferId);

        if (!string.IsNullOrEmpty(request.ShareType))
            query = query.Where(s => s.ShareType.ToString() == request.ShareType);

        query = request.SortBy.ToLower() switch
        {
            "popular" => query.OrderByDescending(s => s.LikeCount).ThenByDescending(s => s.ViewCount),
            "rating" => query.OrderByDescending(s => s.Rating ?? 0),
            _ => query.OrderByDescending(s => s.CreatedUtc)
        };

        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ProductShareListDto(
                s.Id,
                s.UserId,
                s.User!.DisplayName ?? "Unknown",
                s.User.AvatarUrl,
                s.OfferId,
                s.Offer != null && s.Offer.Product != null ? s.Offer.Product.Translations.Select(t => t.Name).FirstOrDefault() : null,
                s.Offer != null && s.Offer.Product != null && s.Offer.Product.PrimaryImage != null ? s.Offer.Product.PrimaryImage.Url : null,
                s.Title,
                s.ShareType.ToString(),
                s.Rating,
                s.ViewCount,
                s.LikeCount,
                s.CommentCount,
                s.CreatedUtc
            ))
            .ToListAsync(ct);

        return new PaginatedSharesResult(items, totalCount, request.Page, request.PageSize, totalPages);
    }
}

public class GetFeedHandler : IRequestHandler<GetFeedQuery, PaginatedSharesResult>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetFeedHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PaginatedSharesResult> Handle(GetFeedQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var query = _db.ProductShares
            .Include(s => s.User)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Product)
                    .ThenInclude(p => p!.Translations)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Product)
                    .ThenInclude(p => p!.PrimaryImage)
            .AsQueryable();

        // If user is logged in, show content from followed users first
        if (userId.HasValue)
        {
            var followingIds = await _db.UserFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowingId)
                .ToListAsync(ct);

            query = query.OrderByDescending(s => followingIds.Contains(s.UserId))
                         .ThenByDescending(s => s.CreatedUtc);
        }
        else
        {
            query = query.OrderByDescending(s => s.CreatedUtc);
        }

        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ProductShareListDto(
                s.Id,
                s.UserId,
                s.User!.DisplayName ?? "Unknown",
                s.User.AvatarUrl,
                s.OfferId,
                s.Offer != null && s.Offer.Product != null ? s.Offer.Product.Translations.Select(t => t.Name).FirstOrDefault() : null,
                s.Offer != null && s.Offer.Product != null && s.Offer.Product.PrimaryImage != null ? s.Offer.Product.PrimaryImage.Url : null,
                s.Title,
                s.ShareType.ToString(),
                s.Rating,
                s.ViewCount,
                s.LikeCount,
                s.CommentCount,
                s.CreatedUtc
            ))
            .ToListAsync(ct);

        return new PaginatedSharesResult(items, totalCount, request.Page, request.PageSize, totalPages);
    }
}

public class GetShareCommentsHandler : IRequestHandler<GetShareCommentsQuery, List<ShareCommentDto>>
{
    private readonly IAppDbContext _db;

    public GetShareCommentsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ShareCommentDto>> Handle(GetShareCommentsQuery request, CancellationToken ct)
    {
        var comments = await _db.ShareComments
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.User)
            .Where(c => c.ShareId == request.ShareId && c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return comments.Select(MapComment).ToList();
    }

    private static ShareCommentDto MapComment(Domain.Entities.Social.ShareComment c)
    {
        return new ShareCommentDto(
            c.Id,
            c.ShareId,
            c.UserId,
            c.User?.DisplayName ?? "Unknown",
            c.User?.AvatarUrl,
            c.ParentCommentId,
            c.Content,
            c.LikeCount,
            c.CreatedUtc,
            c.Replies?.Select(MapComment).ToList()
        );
    }
}

public class GetFollowersHandler : IRequestHandler<GetFollowersQuery, PaginatedFollowersResult>
{
    private readonly IAppDbContext _db;

    public GetFollowersHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedFollowersResult> Handle(GetFollowersQuery request, CancellationToken ct)
    {
        var query = _db.UserFollows
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .Where(f => f.FollowingId == request.UserId)
            .OrderByDescending(f => f.CreatedUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new UserFollowDto(
                f.Id,
                f.FollowerId,
                f.Follower!.DisplayName ?? "Unknown",
                f.Follower.AvatarUrl,
                f.FollowingId,
                f.Following!.DisplayName ?? "Unknown",
                f.Following.AvatarUrl,
                f.CreatedUtc
            ))
            .ToListAsync(ct);

        return new PaginatedFollowersResult(items, totalCount, request.Page, request.PageSize);
    }
}

public class GetFollowingHandler : IRequestHandler<GetFollowingQuery, PaginatedFollowersResult>
{
    private readonly IAppDbContext _db;

    public GetFollowingHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedFollowersResult> Handle(GetFollowingQuery request, CancellationToken ct)
    {
        var query = _db.UserFollows
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .Where(f => f.FollowerId == request.UserId)
            .OrderByDescending(f => f.CreatedUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new UserFollowDto(
                f.Id,
                f.FollowerId,
                f.Follower!.DisplayName ?? "Unknown",
                f.Follower.AvatarUrl,
                f.FollowingId,
                f.Following!.DisplayName ?? "Unknown",
                f.Following.AvatarUrl,
                f.CreatedUtc
            ))
            .ToListAsync(ct);

        return new PaginatedFollowersResult(items, totalCount, request.Page, request.PageSize);
    }
}

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUserProfileHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { request.UserId }, ct);
        if (user == null) return null;

        var followerCount = await _db.UserFollows.CountAsync(f => f.FollowingId == request.UserId, ct);
        var followingCount = await _db.UserFollows.CountAsync(f => f.FollowerId == request.UserId, ct);
        var postCount = await _db.BlogPosts.CountAsync(p => p.AuthorId == request.UserId, ct);
        var shareCount = await _db.ProductShares.CountAsync(s => s.UserId == request.UserId, ct);

        var isFollowing = _currentUser.UserId.HasValue && await _db.UserFollows
            .AnyAsync(f => f.FollowerId == _currentUser.UserId && f.FollowingId == request.UserId, ct);

        return new UserProfileDto(
            user.Id,
            user.DisplayName ?? "Unknown",
            user.AvatarUrl,
            user.Bio,
            followerCount,
            followingCount,
            postCount,
            shareCount,
            isFollowing,
            user.CreatedUtc
        );
    }
}

public class IsFollowingHandler : IRequestHandler<IsFollowingQuery, bool>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public IsFollowingHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(IsFollowingQuery request, CancellationToken ct)
    {
        if (!_currentUser.UserId.HasValue) return false;

        return await _db.UserFollows
            .AnyAsync(f => f.FollowerId == _currentUser.UserId && f.FollowingId == request.UserId, ct);
    }
}
