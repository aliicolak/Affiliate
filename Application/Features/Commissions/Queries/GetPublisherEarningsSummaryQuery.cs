using Application.Abstractions;
using Application.Features.Commissions.DTOs;

namespace Application.Features.Commissions.Queries;

/// <summary>
/// Publisher'ın kazanç özeti
/// </summary>
public sealed record GetPublisherEarningsSummaryQuery(
    long PublisherId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IQuery<EarningsSummaryDto>;
