using Application.Abstractions;

namespace Application.Features.Products.Commands;

public sealed record CreateProductCommand(
    string Slug,
    string? Sku,
    long? BrandId,
    long? DefaultCategoryId,
    string Name,
    string? Description,
    string LanguageCode = "tr"
) : ICommand<long>;
