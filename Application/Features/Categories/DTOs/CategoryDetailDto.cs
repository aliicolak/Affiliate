namespace Application.Features.Categories.DTOs;

public sealed class CategoryDetailDto
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
