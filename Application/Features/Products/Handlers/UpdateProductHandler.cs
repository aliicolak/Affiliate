using Application.Abstractions.Persistence;
using Application.Features.Products.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateProductHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null) return false;

        product.Slug = request.Slug.ToLower().Replace(" ", "-");
        product.Sku = request.Sku;
        product.BrandId = request.BrandId;
        product.DefaultCategoryId = request.DefaultCategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedUtc = DateTime.UtcNow;

        // Update or create translation
        var translation = await _context.ProductTranslations
            .FirstOrDefaultAsync(t => t.ProductId == request.Id && t.LanguageCode == request.LanguageCode, cancellationToken);

        if (translation is not null)
        {
            translation.Name = request.Name;
            translation.Description = request.Description;
        }
        else
        {
            _context.ProductTranslations.Add(new Domain.Entities.Catalog.ProductTranslation
            {
                ProductId = product.Id,
                LanguageCode = request.LanguageCode,
                Name = request.Name,
                Description = request.Description
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
