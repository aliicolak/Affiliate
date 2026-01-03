using Application.Abstractions;

namespace Application.Features.Categories.Commands;

public sealed record CreateCategoryCommand(
    long? ParentId,
    string Slug,
    int SortOrder,
    string Name,
    string? Description,
    string LanguageCode = "tr"
) : ICommand<long>;
