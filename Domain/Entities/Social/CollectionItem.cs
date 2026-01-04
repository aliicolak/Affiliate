using Domain.Common;

namespace Domain.Entities.Social;

public class CollectionItem : BaseEntity
{
    public long CollectionId { get; set; }
    public Collection Collection { get; set; } = default!;

    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
}
