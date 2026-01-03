using Application.Abstractions.Persistence;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Handlers;

public sealed class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDetailDto?>
{
    private readonly IAppDbContext _context;

    public GetCategoryByIdHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDetailDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Translations)
            .Include(c => c.Parent)
                .ThenInclude(p => p!.Translations)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null) return null;

        var translation = category.Translations
            .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
            ?? category.Translations.FirstOrDefault();

        var parentTranslation = category.Parent?.Translations
            .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
            ?? category.Parent?.Translations.FirstOrDefault();

        return new CategoryDetailDto
        {
            Id = category.Id,
            ParentId = category.ParentId,
            ParentName = parentTranslation?.Name,
            Slug = category.Slug,
            Name = translation?.Name ?? "",
            Description = translation?.Description,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            CreatedUtc = category.CreatedUtc,
            UpdatedUtc = category.UpdatedUtc
        };
    }
}
