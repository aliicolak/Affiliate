using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Persistence;
using Domain.Entities.Blog;
using System.Text.RegularExpressions;

namespace Application.Features.Blog.Commands;

public class CreateBlogPostHandler : IRequestHandler<CreateBlogPostCommand, BlogPostDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateBlogPostHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BlogPostDto> Handle(CreateBlogPostCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        
        var slug = GenerateSlug(request.Title);
        
        // Ensure unique slug
        var baseSlug = slug;
        var counter = 1;
        while (await _db.BlogPosts.AnyAsync(p => p.Slug == slug, ct))
        {
            slug = $"{baseSlug}-{counter++}";
        }

        var post = new BlogPost
        {
            AuthorId = userId,
            Title = request.Title,
            Slug = slug,
            Content = request.Content,
            Excerpt = request.Excerpt ?? ExtractExcerpt(request.Content),
            CoverImageUrl = request.CoverImageUrl,
            CategoryId = request.CategoryId,
            Tags = request.Tags,
            Status = request.Publish ? BlogPostStatus.Published : BlogPostStatus.Draft,
            PublishedUtc = request.Publish ? DateTime.UtcNow : null,
            ReadingTimeMinutes = CalculateReadingTime(request.Content)
        };

        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync(ct);

        // Update category post count
        if (post.CategoryId.HasValue)
        {
            var category = await _db.BlogCategories.FindAsync(new object[] { post.CategoryId.Value }, ct);
            if (category != null)
            {
                category.PostCount++;
                await _db.SaveChangesAsync(ct);
            }
        }

        var author = await _db.Users.FindAsync(new object[] { userId }, ct);

        return new BlogPostDto(
            post.Id,
            post.AuthorId,
            author?.DisplayName ?? "Unknown",
            author?.AvatarUrl,
            post.CategoryId,
            null,
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

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }

    private static string ExtractExcerpt(string content, int maxLength = 200)
    {
        var text = Regex.Replace(content, @"<[^>]+>", ""); // Remove HTML
        text = Regex.Replace(text, @"[#*_`]", ""); // Remove markdown
        return text.Length > maxLength ? text[..maxLength] + "..." : text;
    }

    private static int CalculateReadingTime(string content)
    {
        var wordCount = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
    }
}

public class UpdateBlogPostHandler : IRequestHandler<UpdateBlogPostCommand, BlogPostDto?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateBlogPostHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BlogPostDto?> Handle(UpdateBlogPostCommand request, CancellationToken ct)
    {
        var post = await _db.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (post == null) return null;

        // Check ownership or admin
        var userId = _currentUser.UserId;
        if (post.AuthorId != userId && !_currentUser.IsInRole("Admin"))
            throw new UnauthorizedAccessException();

        post.Title = request.Title;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.CoverImageUrl = request.CoverImageUrl;
        post.CategoryId = request.CategoryId;
        post.Tags = request.Tags;
        post.MetaTitle = request.MetaTitle;
        post.MetaDescription = request.MetaDescription;
        post.UpdatedUtc = DateTime.UtcNow;
        post.ReadingTimeMinutes = CalculateReadingTime(request.Content);

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

    private static int CalculateReadingTime(string content)
    {
        var wordCount = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
    }
}

public class DeleteBlogPostHandler : IRequestHandler<DeleteBlogPostCommand, bool>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteBlogPostHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteBlogPostCommand request, CancellationToken ct)
    {
        var post = await _db.BlogPosts.FindAsync(new object[] { request.Id }, ct);
        if (post == null) return false;

        var userId = _currentUser.UserId;
        if (post.AuthorId != userId && !_currentUser.IsInRole("Admin"))
            throw new UnauthorizedAccessException();

        post.DeletedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

public class PublishBlogPostHandler : IRequestHandler<PublishBlogPostCommand, bool>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public PublishBlogPostHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(PublishBlogPostCommand request, CancellationToken ct)
    {
        var post = await _db.BlogPosts.FindAsync(new object[] { request.Id }, ct);
        if (post == null) return false;

        var userId = _currentUser.UserId;
        if (post.AuthorId != userId && !_currentUser.IsInRole("Admin"))
            throw new UnauthorizedAccessException();

        if (request.Publish)
        {
            post.Status = BlogPostStatus.Published;
            post.PublishedUtc ??= DateTime.UtcNow;
        }
        else
        {
            post.Status = BlogPostStatus.Draft;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }
}

// Interface for current user service
public interface ICurrentUserService
{
    long? UserId { get; }
    string? UserName { get; }
    bool IsInRole(string role);
}
