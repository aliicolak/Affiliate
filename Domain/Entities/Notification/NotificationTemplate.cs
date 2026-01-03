using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Notification;

/// <summary>
/// Bildirim şablonu
/// </summary>
public class NotificationTemplate : BaseEntity
{
    /// <summary>
    /// Şablon kodu (benzersiz)
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Bildirim türü
    /// </summary>
    public NotificationType Type { get; set; }
    
    /// <summary>
    /// Başlık şablonu (placeholder destekli)
    /// </summary>
    public string TitleTemplate { get; set; } = string.Empty;
    
    /// <summary>
    /// Mesaj şablonu (placeholder destekli)
    /// </summary>
    public string MessageTemplate { get; set; } = string.Empty;
    
    /// <summary>
    /// E-posta da gönderilsin mi?
    /// </summary>
    public bool SendEmail { get; set; }
    
    /// <summary>
    /// E-posta şablonu
    /// </summary>
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;
}
