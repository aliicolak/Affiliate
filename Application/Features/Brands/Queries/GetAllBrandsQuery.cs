using Application.Abstractions;
using Application.Features.Brands.DTOs;

namespace Application.Features.Brands.Queries;

public sealed record GetAllBrandsQuery(bool IncludeInactive = false) : IQuery<List<BrandDto>>;
