namespace Application.Features.Categories.DTOs;

public sealed class CategoryTreeDto
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new();
}
