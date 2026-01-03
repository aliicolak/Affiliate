using Application.Abstractions;

namespace Application.Features.Brands.Commands;

public sealed record UpdateBrandCommand(
    long Id,
    string Name,
    string Slug,
    long? LogoAssetId,
    bool IsActive
) : ICommand<bool>;
