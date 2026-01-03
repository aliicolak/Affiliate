using Application.Abstractions;

namespace Application.Features.Brands.Commands;

public sealed record CreateBrandCommand(
    string Name,
    string Slug,
    long? LogoAssetId
) : ICommand<long>;
