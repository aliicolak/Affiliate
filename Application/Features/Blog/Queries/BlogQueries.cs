using MediatR;

namespace Application.Features.Blog.Queries;

// Get Blog Post by Id or Slug
public record GetBlogPostQuery(long? Id = null, string? Slug = null) : IRequest<BlogPostDto?>;

// Get Blog Posts (paginated, with filters)
public record GetBlogPostsQuery(
    long? CategoryId = null,
    long? AuthorId = null,
    string? Tag = null,
    string? Search = null,
    bool? IsFeatured = null,
    bool PublishedOnly = true,
    string SortBy = "newest", // newest, popular, views
    int Page = 1,
    int PageSize = 10
) : IRequest<PaginatedBlogPostsResult>;

// Get Blog Categories
public record GetBlogCategoriesQuery(bool IncludeEmpty = false) : IRequest<List<BlogCategoryDto>>;

// Get Blog Comments for a Post
public record GetBlogCommentsQuery(long PostId, int Page = 1, int PageSize = 20) : IRequest<PaginatedCommentsResult>;

// Get User's Blog Posts
public record GetUserBlogPostsQuery(
    long UserId,
    bool IncludeDrafts = false,
    int Page = 1,
    int PageSize = 10
) : IRequest<PaginatedBlogPostsResult>;
