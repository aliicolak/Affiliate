using Application.Abstractions.Persistence;
using Application.Features.Categories.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Handlers;

public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateCategoryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null) return false;

        category.ParentId = request.ParentId;
        category.Slug = request.Slug.ToLower().Replace(" ", "-");
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedUtc = DateTime.UtcNow;

        var translation = await _context.CategoryTranslations
            .FirstOrDefaultAsync(t => t.CategoryId == request.Id && t.LanguageCode == request.LanguageCode, cancellationToken);

        if (translation is not null)
        {
            translation.Name = request.Name;
            translation.Description = request.Description;
        }
        else
        {
            _context.CategoryTranslations.Add(new Domain.Entities.Catalog.CategoryTranslation
            {
                CategoryId = category.Id,
                LanguageCode = request.LanguageCode,
                Name = request.Name,
                Description = request.Description
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
