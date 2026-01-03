using Application.Abstractions;
using Application.Features.AffiliatePrograms.DTOs;

namespace Application.Features.AffiliatePrograms.Queries;

public sealed record GetAffiliateProgramByIdQuery(long Id) : IQuery<AffiliateProgramDto?>;
