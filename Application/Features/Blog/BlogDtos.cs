namespace Application.Features.Blog;

// Blog Post DTOs
public record BlogPostDto(
    long Id,
    long AuthorId,
    string AuthorName,
    string? AuthorAvatarUrl,
    long? CategoryId,
    string? CategoryName,
    string Title,
    string Slug,
    string? Excerpt,
    string Content,
    string? CoverImageUrl,
    string? Tags,
    string Status,
    bool IsFeatured,
    int ViewCount,
    int LikeCount,
    int CommentCount,
    int ReadingTimeMinutes,
    string? MetaTitle,
    string? MetaDescription,
    DateTime CreatedUtc,
    DateTime? PublishedUtc
);

public record BlogPostListDto(
    long Id,
    long AuthorId,
    string AuthorName,
    string? AuthorAvatarUrl,
    string? CategoryName,
    string Title,
    string Slug,
    string? Excerpt,
    string? CoverImageUrl,
    string? Tags,
    bool IsFeatured,
    int ViewCount,
    int LikeCount,
    int CommentCount,
    int ReadingTimeMinutes,
    DateTime? PublishedUtc
);

public record BlogCommentDto(
    long Id,
    long PostId,
    long UserId,
    string UserName,
    string? UserAvatarUrl,
    long? ParentCommentId,
    string Content,
    int LikeCount,
    DateTime CreatedUtc,
    List<BlogCommentDto>? Replies
);

public record BlogCategoryDto(
    long Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    int PostCount,
    int DisplayOrder,
    bool IsActive
);

// Paginated Results
public record PaginatedBlogPostsResult(
    List<BlogPostListDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record PaginatedCommentsResult(
    List<BlogCommentDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
