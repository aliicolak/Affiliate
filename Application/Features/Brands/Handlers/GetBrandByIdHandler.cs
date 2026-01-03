using Application.Abstractions.Persistence;
using Application.Features.Brands.DTOs;
using Application.Features.Brands.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Brands.Handlers;

public sealed class GetBrandByIdHandler : IRequestHandler<GetBrandByIdQuery, BrandDto?>
{
    private readonly IAppDbContext _context;

    public GetBrandByIdHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<BrandDto?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands
            .AsNoTracking()
            .Include(b => b.Logo)
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (brand is null) return null;

        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Slug = brand.Slug,
            LogoUrl = brand.Logo?.Url,
            IsActive = brand.IsActive,
            ProductCount = brand.Products.Count,
            CreatedUtc = brand.CreatedUtc
        };
    }
}
