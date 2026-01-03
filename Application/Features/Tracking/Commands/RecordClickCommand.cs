using Application.Abstractions;
using Domain.Enums;

namespace Application.Features.Tracking.Commands;

/// <summary>
/// Yeni tıklama kaydı komutu
/// </summary>
public sealed record RecordClickCommand(
    long OfferId,
    long? PublisherId,
    string TrackingCode,
    string IpAddress,
    string? UserAgent,
    string? Referrer,
    string? SubId1 = null,
    string? SubId2 = null,
    string? SubId3 = null
) : ICommand<RecordClickResult>;

/// <summary>
/// Tıklama kaydı sonucu
/// </summary>
public sealed record RecordClickResult(
    long ClickId,
    string RedirectUrl,
    bool IsNewSession
);
