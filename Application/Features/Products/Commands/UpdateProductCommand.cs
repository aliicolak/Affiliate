using Application.Abstractions;

namespace Application.Features.Products.Commands;

public sealed record UpdateProductCommand(
    long Id,
    string Slug,
    string? Sku,
    long? BrandId,
    long? DefaultCategoryId,
    bool IsActive,
    string Name,
    string? Description,
    string LanguageCode = "tr"
) : ICommand<bool>;
