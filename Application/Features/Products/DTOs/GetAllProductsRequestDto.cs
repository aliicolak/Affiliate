namespace Application.Features.Products.DTOs;

public sealed class GetAllProductsRequestDto
{
    public long? BrandId { get; set; }
    public long? CategoryId { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "id";
    public string? SortOrder { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string LanguageCode { get; set; } = "tr";
}
