using Application.Abstractions;
using Application.Features.Commissions.DTOs;

namespace Application.Features.Commissions.Queries;

/// <summary>
/// Publisher'ın komisyonlarını getir
/// </summary>
public sealed record GetPublisherCommissionsQuery(
    long PublisherId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 50
) : IQuery<PagedCommissionsDto>;
