using Application.Abstractions.Persistence;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers;

public sealed class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto?>
{
    private readonly IAppDbContext _context;

    public GetProductByIdHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.DefaultCategory)
                .ThenInclude(c => c!.Translations)
            .Include(p => p.Translations)
            .Include(p => p.PrimaryImage)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null) return null;

        var translation = product.Translations
            .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
            ?? product.Translations.FirstOrDefault();

        var categoryTranslation = product.DefaultCategory?.Translations
            .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
            ?? product.DefaultCategory?.Translations.FirstOrDefault();

        return new ProductDetailDto
        {
            Id = product.Id,
            Slug = product.Slug,
            Sku = product.Sku,
            Name = translation?.Name ?? "",
            Description = translation?.Description,
            BrandId = product.BrandId,
            BrandName = product.Brand?.Name,
            DefaultCategoryId = product.DefaultCategoryId,
            CategoryName = categoryTranslation?.Name,
            PrimaryImageUrl = product.PrimaryImage?.Url,
            RatingAvg = product.RatingAvg,
            RatingCount = product.RatingCount,
            IsActive = product.IsActive,
            ReleasedAt = product.ReleasedAt,
            CreatedUtc = product.CreatedUtc,
            UpdatedUtc = product.UpdatedUtc
        };
    }
}
