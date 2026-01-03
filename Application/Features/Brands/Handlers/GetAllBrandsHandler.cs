using Application.Abstractions.Persistence;
using Application.Features.Brands.DTOs;
using Application.Features.Brands.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Brands.Handlers;

public sealed class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, List<BrandDto>>
{
    private readonly IAppDbContext _context;

    public GetAllBrandsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Brands
            .AsNoTracking()
            .Include(b => b.Logo)
            .Include(b => b.Products)
            .AsQueryable();

        if (!request.IncludeInactive)
            query = query.Where(b => b.IsActive);

        return await query
            .OrderBy(b => b.Name)
            .Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Slug = b.Slug,
                LogoUrl = b.Logo != null ? b.Logo.Url : null,
                IsActive = b.IsActive,
                ProductCount = b.Products.Count,
                CreatedUtc = b.CreatedUtc
            })
            .ToListAsync(cancellationToken);
    }
}
