using Domain.Entities.Identity;
using Domain.Entities.Affiliate;

namespace Domain.Entities.Social;

/// <summary>
/// User's product/offer share - review, recommendation, showcase
/// </summary>
public class ProductShare
{
    public long Id { get; set; }
    
    // Author
    public long UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    // Shared content (either Product or Offer)
    public long? OfferId { get; set; }
    public Offer? Offer { get; set; }
    
    // Content
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ShareType ShareType { get; set; } = ShareType.Recommendation;
    
    // Rating (1-5 stars, optional)
    public int? Rating { get; set; }
    
    // Pros and Cons (for reviews)
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    
    // Media
    public string? ImageUrl { get; set; }
    
    // Metrics
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    
    // Dates
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
    
    // Navigation
    public ICollection<ShareComment> Comments { get; set; } = new List<ShareComment>();
}

public enum ShareType
{
    Recommendation = 0,
    Review = 1,
    Showcase = 2,
    Comparison = 3
}
