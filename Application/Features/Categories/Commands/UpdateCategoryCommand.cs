using Application.Abstractions;

namespace Application.Features.Categories.Commands;

public sealed record UpdateCategoryCommand(
    long Id,
    long? ParentId,
    string Slug,
    int SortOrder,
    bool IsActive,
    string Name,
    string? Description,
    string LanguageCode = "tr"
) : ICommand<bool>;
