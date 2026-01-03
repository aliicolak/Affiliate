using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Domain.Entities.Notification;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Bildirim servisi implementasyonu
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly IAppDbContext _context;

    public NotificationService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task SendAsync(
        long userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
        
        // TODO: E-posta gönderimi (IEmailSender ile)
    }

    public async Task SendFromTemplateAsync(
        long userId,
        string templateCode,
        Dictionary<string, string> placeholders,
        string? actionUrl = null,
        string? relatedEntityType = null,
        long? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        var template = await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Code == templateCode && t.IsActive, cancellationToken);

        if (template is null)
        {
            // Template bulunamazsa basit bildirim gönder
            await SendAsync(userId, NotificationType.Info, "Notification", templateCode, 
                actionUrl, relatedEntityType, relatedEntityId, cancellationToken);
            return;
        }

        var title = ReplacePlaceholders(template.TitleTemplate, placeholders);
        var message = ReplacePlaceholders(template.MessageTemplate, placeholders);

        await SendAsync(userId, template.Type, title, message, 
            actionUrl, relatedEntityType, relatedEntityId, cancellationToken);
    }

    private static string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
    {
        var result = template;
        foreach (var (key, value) in placeholders)
        {
            result = result.Replace($"{{{{{key}}}}}", value);
        }
        return result;
    }
}
