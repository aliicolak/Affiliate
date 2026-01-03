namespace Application.Features.Products.DTOs;

public sealed class ProductListDto
{
    public long Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? BrandName { get; set; }
    public string? CategoryName { get; set; }
    public decimal RatingAvg { get; set; }
    public int RatingCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedUtc { get; set; }
}
