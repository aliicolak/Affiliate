using Application.Abstractions;
using Application.Common.Responses;
using Application.Features.Products.DTOs;

namespace Application.Features.Products.Queries;

public sealed record GetAllProductsQuery(GetAllProductsRequestDto RequestDto) 
    : IQuery<PagedResponse<ProductListDto>>;
