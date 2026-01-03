namespace Application.Features.Wishlists.DTOs;

public sealed class WishlistDetailDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedUtc { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
}

public sealed class WishlistItemDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public DateTime AddedUtc { get; set; }
}
