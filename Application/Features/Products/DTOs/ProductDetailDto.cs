namespace Application.Features.Products.DTOs;

public sealed class ProductDetailDto
{
    public long Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? BrandId { get; set; }
    public string? BrandName { get; set; }
    public long? DefaultCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public decimal RatingAvg { get; set; }
    public int RatingCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
