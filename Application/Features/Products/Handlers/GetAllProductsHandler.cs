using Application.Abstractions.Persistence;
using Application.Common.Responses;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers;

public sealed class GetAllProductsHandler 
    : IRequestHandler<GetAllProductsQuery, PagedResponse<ProductListDto>>
{
    private readonly IAppDbContext _context;

    public GetAllProductsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ProductListDto>> Handle(
        GetAllProductsQuery request, 
        CancellationToken cancellationToken)
    {
        var dto = request.RequestDto;

        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.DefaultCategory)
            .Include(p => p.Translations)
            .AsQueryable();

        // === Filters ===
        if (dto.BrandId is not null)
            query = query.Where(p => p.BrandId == dto.BrandId);

        if (dto.CategoryId is not null)
            query = query.Where(p => p.DefaultCategoryId == dto.CategoryId);

        if (dto.IsActive is not null)
            query = query.Where(p => p.IsActive == dto.IsActive);

        if (!string.IsNullOrWhiteSpace(dto.Search))
            query = query.Where(p =>
                p.Slug.Contains(dto.Search) ||
                p.Translations.Any(t => t.Name.Contains(dto.Search)));

        // === Count ===
        var totalCount = await query.CountAsync(cancellationToken);

        // === Sorting ===
        query = dto.SortOrder?.ToLower() == "desc"
            ? dto.SortBy?.ToLower() switch
            {
                "name" => query.OrderByDescending(p => p.Translations.FirstOrDefault()!.Name),
                "createdutc" => query.OrderByDescending(p => p.CreatedUtc),
                "rating" => query.OrderByDescending(p => p.RatingAvg),
                _ => query.OrderByDescending(p => p.Id)
            }
            : dto.SortBy?.ToLower() switch
            {
                "name" => query.OrderBy(p => p.Translations.FirstOrDefault()!.Name),
                "createdutc" => query.OrderBy(p => p.CreatedUtc),
                "rating" => query.OrderBy(p => p.RatingAvg),
                _ => query.OrderBy(p => p.Id)
            };

        // === Pagination ===
        query = query
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize);

        // === Projection ===
        var items = await query
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                Slug = p.Slug,
                Sku = p.Sku,
                Name = p.Translations
                    .Where(t => t.LanguageCode == dto.LanguageCode)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? p.Translations.Select(t => t.Name).FirstOrDefault() ?? "",
                BrandName = p.Brand != null ? p.Brand.Name : null,
                CategoryName = p.DefaultCategory != null 
                    ? p.DefaultCategory.Translations
                        .Where(t => t.LanguageCode == dto.LanguageCode)
                        .Select(t => t.Name)
                        .FirstOrDefault() 
                    : null,
                RatingAvg = p.RatingAvg,
                RatingCount = p.RatingCount,
                IsActive = p.IsActive,
                CreatedUtc = p.CreatedUtc
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((decimal)totalCount / dto.PageSize);

        return new PagedResponse<ProductListDto>
        {
            Data = items,
            Meta = new PagedResponse<ProductListDto>.MetaData
            {
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            }
        };
    }
}
