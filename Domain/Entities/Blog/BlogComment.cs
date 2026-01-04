using Domain.Entities.Identity;

namespace Domain.Entities.Blog;

/// <summary>
/// Blog post comment with nested replies support
/// </summary>
public class BlogComment
{
    public long Id { get; set; }
    
    // Post
    public long PostId { get; set; }
    public BlogPost? Post { get; set; }
    
    // Author
    public long UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    // Parent comment (for nested replies)
    public long? ParentCommentId { get; set; }
    public BlogComment? ParentComment { get; set; }
    
    // Content
    public string Content { get; set; } = string.Empty;
    
    // Status
    public bool IsApproved { get; set; } = true;
    public bool IsEdited { get; set; }
    
    // Metrics
    public int LikeCount { get; set; }
    
    // Dates
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
    
    // Navigation
    public ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();
}
