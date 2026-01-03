using Application.Abstractions.Persistence;
using Application.Features.Categories.Commands;
using Domain.Entities.Catalog;
using MediatR;

namespace Application.Features.Categories.Handlers;

public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, long>
{
    private readonly IAppDbContext _context;

    public CreateCategoryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            ParentId = request.ParentId,
            Slug = request.Slug.ToLower().Replace(" ", "-"),
            SortOrder = request.SortOrder,
            IsActive = true
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        var translation = new CategoryTranslation
        {
            CategoryId = category.Id,
            LanguageCode = request.LanguageCode,
            Name = request.Name,
            Description = request.Description
        };

        _context.CategoryTranslations.Add(translation);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
