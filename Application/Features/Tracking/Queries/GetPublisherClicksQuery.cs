using Application.Abstractions;
using Application.Features.Tracking.DTOs;

namespace Application.Features.Tracking.Queries;

/// <summary>
/// Publisher'a ait tıklama listesi
/// </summary>
public sealed record GetPublisherClicksQuery(
    long PublisherId,
    DateTime? StartDate,
    DateTime? EndDate,
    int Page = 1,
    int PageSize = 50
) : IQuery<PagedClicksDto>;
