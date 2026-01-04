using MediatR;

namespace Application.Features.Blog.Commands;

// Create Blog Post
public record CreateBlogPostCommand(
    string Title,
    string Content,
    string? Excerpt,
    string? CoverImageUrl,
    long? CategoryId,
    string? Tags,
    bool Publish = false
) : IRequest<BlogPostDto>;

// Update Blog Post
public record UpdateBlogPostCommand(
    long Id,
    string Title,
    string Content,
    string? Excerpt,
    string? CoverImageUrl,
    long? CategoryId,
    string? Tags,
    string? MetaTitle,
    string? MetaDescription
) : IRequest<BlogPostDto?>;

// Delete Blog Post
public record DeleteBlogPostCommand(long Id) : IRequest<bool>;

// Publish/Unpublish Blog Post
public record PublishBlogPostCommand(long Id, bool Publish) : IRequest<bool>;

// Create Blog Comment
public record CreateBlogCommentCommand(
    long PostId,
    string Content,
    long? ParentCommentId = null
) : IRequest<BlogCommentDto>;

// Delete Blog Comment
public record DeleteBlogCommentCommand(long Id) : IRequest<bool>;

// Create Blog Category (Admin)
public record CreateBlogCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    int DisplayOrder = 0
) : IRequest<BlogCategoryDto>;

// Update Blog Category (Admin)
public record UpdateBlogCategoryCommand(
    long Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    int DisplayOrder,
    bool IsActive
) : IRequest<BlogCategoryDto?>;
