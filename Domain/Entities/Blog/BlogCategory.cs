namespace Domain.Entities.Blog;

/// <summary>
/// Blog post category
/// </summary>
public class BlogCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int PostCount { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public ICollection<BlogPost> Posts { get; set; } = new List<BlogPost>();
}
