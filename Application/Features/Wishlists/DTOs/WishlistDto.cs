namespace Application.Features.Wishlists.DTOs;

public sealed class WishlistDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedUtc { get; set; }
}
