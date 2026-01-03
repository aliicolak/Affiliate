using Domain.Enums;

namespace Application.Abstractions.Services;

/// <summary>
/// Bildirim servisi
/// In-app ve e-posta bildirimleri gönderir
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Kullanıcıya bildirim gönder
    /// </summary>
    Task SendAsync(
        long userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Şablon kullanarak bildirim gönder
    /// </summary>
    Task SendFromTemplateAsync(
        long userId,
        string templateCode,
        Dictionary<string, string> placeholders,
        string? actionUrl = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default);
}
