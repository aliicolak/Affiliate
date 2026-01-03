using Application.Abstractions;
using Application.Features.Categories.DTOs;

namespace Application.Features.Categories.Queries;

public sealed record GetCategoryByIdQuery(long Id, string LanguageCode = "tr") 
    : IQuery<CategoryDetailDto?>;
