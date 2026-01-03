using Application.Abstractions;
using Application.Features.Publishers.DTOs;

namespace Application.Features.Publishers.Queries;

/// <summary>
/// Publisher dashboard verileri
/// </summary>
public sealed record GetPublisherDashboardQuery(long UserId) : IQuery<PublisherDashboardDto>;
