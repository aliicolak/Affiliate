using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities.Social;

public class Collection : BaseEntity
{
    public long UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsDefault { get; set; }

    public ICollection<CollectionItem> Items { get; set; } = new List<CollectionItem>();
}
