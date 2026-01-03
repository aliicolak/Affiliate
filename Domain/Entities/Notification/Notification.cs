using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Notification;

/// <summary>
/// Kullanıcı bildirimi
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// Hedef kullanıcı
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Bildirim türü
    /// </summary>
    public NotificationType Type { get; set; }
    
    /// <summary>
    /// Başlık
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Mesaj içeriği
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Okundu mu?
    /// </summary>
    public bool IsRead { get; set; }
    
    /// <summary>
    /// Okunma zamanı
    /// </summary>
    public DateTime? ReadUtc { get; set; }
    
    /// <summary>
    /// Aksiyon URL'i (opsiyonel)
    /// </summary>
    public string? ActionUrl { get; set; }
    
    /// <summary>
    /// İlişkili entity tipi (opsiyonel)
    /// </summary>
    public string? RelatedEntityType { get; set; }
    
    /// <summary>
    /// İlişkili entity ID'si (opsiyonel)
    /// </summary>
    public long? RelatedEntityId { get; set; }
}
