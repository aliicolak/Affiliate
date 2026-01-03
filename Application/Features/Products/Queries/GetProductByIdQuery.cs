using Application.Abstractions;
using Application.Features.Products.DTOs;

namespace Application.Features.Products.Queries;

public sealed record GetProductByIdQuery(long Id, string LanguageCode = "tr") 
    : IQuery<ProductDetailDto?>;
