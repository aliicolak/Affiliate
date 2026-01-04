using Domain.Entities.Identity;

namespace Domain.Entities.Social;

/// <summary>
/// Comment on a product share
/// </summary>
public class ShareComment
{
    public long Id { get; set; }
    
    // Parent share
    public long ShareId { get; set; }
    public ProductShare? Share { get; set; }
    
    // Author
    public long UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    // Parent comment for nested replies
    public long? ParentCommentId { get; set; }
    public ShareComment? ParentComment { get; set; }
    
    // Content
    public string Content { get; set; } = string.Empty;
    
    // Metrics
    public int LikeCount { get; set; }
    
    // Dates
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
    
    // Navigation
    public ICollection<ShareComment> Replies { get; set; } = new List<ShareComment>();
}
