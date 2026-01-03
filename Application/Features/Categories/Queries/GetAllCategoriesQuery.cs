using Application.Abstractions;
using Application.Features.Categories.DTOs;

namespace Application.Features.Categories.Queries;

public sealed record GetAllCategoriesQuery(string LanguageCode = "tr", bool IncludeInactive = false) 
    : IQuery<List<CategoryTreeDto>>;
