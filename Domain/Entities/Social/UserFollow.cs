using Domain.Entities.Identity;

namespace Domain.Entities.Social;

/// <summary>
/// User follow relationship
/// </summary>
public class UserFollow
{
    public long Id { get; set; }
    
    /// <summary>
    /// The user who is following
    /// </summary>
    public long FollowerId { get; set; }
    public ApplicationUser? Follower { get; set; }
    
    /// <summary>
    /// The user being followed
    /// </summary>
    public long FollowingId { get; set; }
    public ApplicationUser? Following { get; set; }
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
