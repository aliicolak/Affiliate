using Domain.Common;

namespace Domain.Entities.Affiliate;

/// <summary>
/// Represents a merchant/vendor in the affiliate marketplace.
/// </summary>
public class Merchant : BaseEntity
{
    /// <summary>
    /// Merchant display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-safe slug for the merchant.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Merchant's website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Alias for Website (backward compatibility).
    /// </summary>
    public string? WebsiteUrl { get => Website; set => Website = value; }

    /// <summary>
    /// Merchant description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the merchant is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Offers from this merchant.
    /// </summary>
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}

