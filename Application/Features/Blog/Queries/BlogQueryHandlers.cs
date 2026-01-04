using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Persistence;

namespace Application.Features.Blog.Queries;

public class GetBlogPostHandler : IRequestHandler<GetBlogPostQuery, BlogPostDto?>
{
    private readonly IAppDbContext _db;

    public GetBlogPostHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BlogPostDto?> Handle(GetBlogPostQuery request, CancellationToken ct)
    {
        var query = _db.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .AsQueryable();

        Domain.Entities.Blog.BlogPost? post = null;

        if (request.Id.HasValue)
        {
            post = await query.FirstOrDefaultAsync(p => p.Id == request.Id.Value, ct);
        }
        else if (!string.IsNullOrEmpty(request.Slug))
        {
            post = await query.FirstOrDefaultAsync(p => p.Slug == request.Slug, ct);
        }

        if (post == null) return null;

        // Increment view count
        post.ViewCount++;
        await _db.SaveChangesAsync(ct);

        return new BlogPostDto(
            post.Id,
            post.AuthorId,
            post.Author?.DisplayName ?? "Unknown",
            post.Author?.AvatarUrl,
            post.CategoryId,
            post.Category?.Name,
            post.Title,
            post.Slug,
            post.Excerpt,
            post.Content,
            post.CoverImageUrl,
            post.Tags,
            post.Status.ToString(),
            post.IsFeatured,
            post.ViewCount,
            post.LikeCount,
            post.CommentCount,
            post.ReadingTimeMinutes,
            post.MetaTitle,
            post.MetaDescription,
            post.CreatedUtc,
            post.PublishedUtc
        );
    }
}

public class GetBlogPostsHandler : IRequestHandler<GetBlogPostsQuery, PaginatedBlogPostsResult>
{
    private readonly IAppDbContext _db;

    public GetBlogPostsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedBlogPostsResult> Handle(GetBlogPostsQuery request, CancellationToken ct)
    {
        var query = _db.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .AsQueryable();

        // Filters
        if (request.PublishedOnly)
            query = query.Where(p => p.Status == Domain.Entities.Blog.BlogPostStatus.Published);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.AuthorId.HasValue)
            query = query.Where(p => p.AuthorId == request.AuthorId);

        if (request.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == request.IsFeatured);

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(p => p.Title.Contains(request.Search) || 
                                     (p.Content != null && p.Content.Contains(request.Search)));

        if (!string.IsNullOrEmpty(request.Tag))
            query = query.Where(p => p.Tags != null && p.Tags.Contains(request.Tag));

        // Sorting
        query = request.SortBy.ToLower() switch
        {
            "popular" => query.OrderByDescending(p => p.LikeCount).ThenByDescending(p => p.ViewCount),
            "views" => query.OrderByDescending(p => p.ViewCount),
            _ => query.OrderByDescending(p => p.PublishedUtc ?? p.CreatedUtc)
        };

        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var posts = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new BlogPostListDto(
                p.Id,
                p.AuthorId,
                p.Author!.DisplayName ?? "Unknown",
                p.Author.AvatarUrl,
                p.Category != null ? p.Category.Name : null,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.CoverImageUrl,
                p.Tags,
                p.IsFeatured,
                p.ViewCount,
                p.LikeCount,
                p.CommentCount,
                p.ReadingTimeMinutes,
                p.PublishedUtc
            ))
            .ToListAsync(ct);

        return new PaginatedBlogPostsResult(posts, totalCount, request.Page, request.PageSize, totalPages);
    }
}

public class GetBlogCategoriesHandler : IRequestHandler<GetBlogCategoriesQuery, List<BlogCategoryDto>>
{
    private readonly IAppDbContext _db;

    public GetBlogCategoriesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<BlogCategoryDto>> Handle(GetBlogCategoriesQuery request, CancellationToken ct)
    {
        var query = _db.BlogCategories.Where(c => c.IsActive);

        if (!request.IncludeEmpty)
            query = query.Where(c => c.PostCount > 0);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new BlogCategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.IconUrl,
                c.PostCount,
                c.DisplayOrder,
                c.IsActive
            ))
            .ToListAsync(ct);
    }
}

public class GetBlogCommentsHandler : IRequestHandler<GetBlogCommentsQuery, PaginatedCommentsResult>
{
    private readonly IAppDbContext _db;

    public GetBlogCommentsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedCommentsResult> Handle(GetBlogCommentsQuery request, CancellationToken ct)
    {
        var query = _db.BlogComments
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.User)
            .Where(c => c.PostId == request.PostId && c.ParentCommentId == null && c.IsApproved)
            .OrderByDescending(c => c.CreatedUtc);

        var totalCount = await query.CountAsync(ct);

        var comments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var result = comments.Select(MapComment).ToList();

        return new PaginatedCommentsResult(result, totalCount, request.Page, request.PageSize);
    }

    private static BlogCommentDto MapComment(Domain.Entities.Blog.BlogComment c)
    {
        return new BlogCommentDto(
            c.Id,
            c.PostId,
            c.UserId,
            c.User?.DisplayName ?? "Unknown",
            c.User?.AvatarUrl,
            c.ParentCommentId,
            c.Content,
            c.LikeCount,
            c.CreatedUtc,
            c.Replies?.Where(r => r.IsApproved).Select(MapComment).ToList()
        );
    }
}

public class GetUserBlogPostsHandler : IRequestHandler<GetUserBlogPostsQuery, PaginatedBlogPostsResult>
{
    private readonly IAppDbContext _db;

    public GetUserBlogPostsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedBlogPostsResult> Handle(GetUserBlogPostsQuery request, CancellationToken ct)
    {
        var query = _db.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.AuthorId == request.UserId);

        if (!request.IncludeDrafts)
            query = query.Where(p => p.Status == Domain.Entities.Blog.BlogPostStatus.Published);

        query = query.OrderByDescending(p => p.CreatedUtc);

        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var posts = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new BlogPostListDto(
                p.Id,
                p.AuthorId,
                p.Author!.DisplayName ?? "Unknown",
                p.Author.AvatarUrl,
                p.Category != null ? p.Category.Name : null,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.CoverImageUrl,
                p.Tags,
                p.IsFeatured,
                p.ViewCount,
                p.LikeCount,
                p.CommentCount,
                p.ReadingTimeMinutes,
                p.PublishedUtc
            ))
            .ToListAsync(ct);

        return new PaginatedBlogPostsResult(posts, totalCount, request.Page, request.PageSize, totalPages);
    }
}
