using Application.Abstractions;
using Application.Features.AffiliatePrograms.DTOs;

namespace Application.Features.AffiliatePrograms.Queries;

public sealed record GetAllAffiliateProgramsQuery(long? MerchantId = null) : IQuery<List<AffiliateProgramDto>>;
