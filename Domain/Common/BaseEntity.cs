namespace Domain.Common;

/// <summary>
/// Base entity with common audit properties.
/// All entities inherit from this for consistent tracking.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp (UTC).
    /// </summary>
    public DateTime? UpdatedUtc { get; set; }

    /// <summary>
    /// Soft delete timestamp (UTC). Null means not deleted.
    /// </summary>
    public DateTime? DeletedUtc { get; set; }

    /// <summary>
    /// Indicates whether the entity is soft-deleted.
    /// </summary>
    public bool IsDeleted => DeletedUtc.HasValue;
}

