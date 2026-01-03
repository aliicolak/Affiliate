using Application.Abstractions.Persistence;
using Application.Features.Brands.Commands;
using Domain.Entities.Catalog;
using Domain.Services;
using MediatR;

namespace Application.Features.Brands.Handlers;

/// <summary>
/// Handles the creation of a new brand.
/// </summary>
public sealed class CreateBrandHandler : IRequestHandler<CreateBrandCommand, long>
{
    private readonly IAppDbContext _context;

    public CreateBrandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = new Brand
        {
            Name = request.Name,
            Slug = SlugGenerator.Generate(request.Slug),
            LogoAssetId = request.LogoAssetId,
            IsActive = true
        };

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}

