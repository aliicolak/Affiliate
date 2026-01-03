using Application.Abstractions;
using Application.Common.Responses;
using Application.Features.Tracking.DTOs;

namespace Application.Features.Tracking.Queries;

/// <summary>
/// Tıklama istatistikleri sorgusu
/// </summary>
public sealed record GetClickStatsQuery(
    long? PublisherId,
    long? MerchantId,
    long? OfferId,
    DateTime? StartDate,
    DateTime? EndDate,
    string? GroupBy = "day" // day, week, month, country, device
) : IQuery<ClickStatsDto>;
