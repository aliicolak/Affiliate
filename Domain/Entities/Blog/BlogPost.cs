using Domain.Entities.Identity;

namespace Domain.Entities.Blog;

/// <summary>
/// Blog post entity - for affiliate marketing content
/// </summary>
public class BlogPost
{
    public long Id { get; set; }
    
    // Author
    public long AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }
    
    // Category
    public long? CategoryId { get; set; }
    public BlogCategory? Category { get; set; }
    
    // Content
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    
    // Tags (comma-separated or JSON array)
    public string? Tags { get; set; }
    
    // Status
    public BlogPostStatus Status { get; set; } = BlogPostStatus.Draft;
    public bool IsFeatured { get; set; }
    
    // Metrics
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    
    // Reading time (estimated minutes)
    public int ReadingTimeMinutes { get; set; }
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    
    // Dates
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
    public DateTime? PublishedUtc { get; set; }
    public DateTime? DeletedUtc { get; set; }
    
    // Navigation
    public ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();
}

public enum BlogPostStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}
