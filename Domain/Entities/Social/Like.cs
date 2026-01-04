using Domain.Entities.Identity;

namespace Domain.Entities.Social;

/// <summary>
/// Generic like entity for blog posts, shares, comments etc.
/// </summary>
public class Like
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Type of entity being liked: BlogPost, ProductShare, BlogComment, ShareComment
    /// </summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the entity being liked
    /// </summary>
    public long EntityId { get; set; }
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
