using Application.Abstractions;
using Application.Features.Brands.DTOs;

namespace Application.Features.Brands.Queries;

public sealed record GetBrandByIdQuery(long Id) : IQuery<BrandDto?>;
