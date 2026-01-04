using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Persistence;
using Domain.Entities.Blog;

namespace Application.Features.Blog.Commands;

public class CreateBlogCommentHandler : IRequestHandler<CreateBlogCommentCommand, BlogCommentDto>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateBlogCommentHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BlogCommentDto> Handle(CreateBlogCommentCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

        var post = await _db.BlogPosts.FindAsync(new object[] { request.PostId }, ct);
        if (post == null)
            throw new ArgumentException("Post not found");

        var comment = new BlogComment
        {
            PostId = request.PostId,
            UserId = userId,
            ParentCommentId = request.ParentCommentId,
            Content = request.Content
        };

        _db.BlogComments.Add(comment);
        post.CommentCount++;
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync(new object[] { userId }, ct);

        return new BlogCommentDto(
            comment.Id,
            comment.PostId,
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

public class DeleteBlogCommentHandler : IRequestHandler<DeleteBlogCommentCommand, bool>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteBlogCommentHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteBlogCommentCommand request, CancellationToken ct)
    {
        var comment = await _db.BlogComments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (comment == null) return false;

        var userId = _currentUser.UserId;
        if (comment.UserId != userId && !_currentUser.IsInRole("Admin"))
            throw new UnauthorizedAccessException();

        _db.BlogComments.Remove(comment);
        
        if (comment.Post != null)
            comment.Post.CommentCount = Math.Max(0, comment.Post.CommentCount - 1);

        await _db.SaveChangesAsync(ct);
        return true;
    }
}

public class CreateBlogCategoryHandler : IRequestHandler<CreateBlogCategoryCommand, BlogCategoryDto>
{
    private readonly IAppDbContext _db;

    public CreateBlogCategoryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BlogCategoryDto> Handle(CreateBlogCategoryCommand request, CancellationToken ct)
    {
        var category = new BlogCategory
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            IconUrl = request.IconUrl,
            DisplayOrder = request.DisplayOrder
        };

        _db.BlogCategories.Add(category);
        await _db.SaveChangesAsync(ct);

        return new BlogCategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IconUrl,
            category.PostCount,
            category.DisplayOrder,
            category.IsActive
        );
    }
}

public class UpdateBlogCategoryHandler : IRequestHandler<UpdateBlogCategoryCommand, BlogCategoryDto?>
{
    private readonly IAppDbContext _db;

    public UpdateBlogCategoryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BlogCategoryDto?> Handle(UpdateBlogCategoryCommand request, CancellationToken ct)
    {
        var category = await _db.BlogCategories.FindAsync(new object[] { request.Id }, ct);
        if (category == null) return null;

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);

        return new BlogCategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IconUrl,
            category.PostCount,
            category.DisplayOrder,
            category.IsActive
        );
    }
}
