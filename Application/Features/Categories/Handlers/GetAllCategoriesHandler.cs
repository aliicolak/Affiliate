using Application.Abstractions.Persistence;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Handlers;

public sealed class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryTreeDto>>
{
    private readonly IAppDbContext _context;

    public GetAllCategoriesHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryTreeDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Include(c => c.Translations)
            .AsQueryable();

        if (!request.IncludeInactive)
            query = query.Where(c => c.IsActive);

        var allCategories = await query
            .Select(c => new CategoryTreeDto
            {
                Id = c.Id,
                ParentId = c.ParentId,
                Slug = c.Slug,
                Name = c.Translations
                    .Where(t => t.LanguageCode == request.LanguageCode)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? c.Translations.Select(t => t.Name).FirstOrDefault() ?? "",
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            })
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        // Build tree structure
        var lookup = allCategories.ToLookup(c => c.ParentId);
        
        foreach (var category in allCategories)
        {
            category.Children = lookup[category.Id].ToList();
        }

        // Return only root categories (ParentId is null)
        return allCategories.Where(c => c.ParentId is null).ToList();
    }
}
