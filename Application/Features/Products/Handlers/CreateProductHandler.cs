using Application.Abstractions.Persistence;
using Application.Features.Products.Commands;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Products.Handlers;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, long>
{
    private readonly IAppDbContext _context;

    public CreateProductHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Slug = request.Slug.ToLower().Replace(" ", "-"),
            Sku = request.Sku,
            BrandId = request.BrandId,
            DefaultCategoryId = request.DefaultCategoryId,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Add translation
        var translation = new ProductTranslation
        {
            ProductId = product.Id,
            LanguageCode = request.LanguageCode,
            Name = request.Name,
            Description = request.Description
        };

        _context.ProductTranslations.Add(translation);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
