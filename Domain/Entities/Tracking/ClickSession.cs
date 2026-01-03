using Domain.Common;

namespace Domain.Entities.Tracking;

/// <summary>
/// Tıklama oturumu - aynı kullanıcının belirli süre içindeki tıklamalarını gruplar
/// Cookie-based tracking için kullanılır
/// </summary>
public class ClickSession : BaseEntity
{
    /// <summary>
    /// Benzersiz session ID (cookie'de saklanır)
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Publisher ID
    /// </summary>
    public long? PublisherId { get; set; }
    
    /// <summary>
    /// İlk tıklamanın IP adresi
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Session başlangıç zamanı
    /// </summary>
    public DateTime StartedUtc { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Son aktivite zamanı
    /// </summary>
    public DateTime LastActivityUtc { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Session sona erme zamanı (cookie expiry)
    /// </summary>
    public DateTime ExpiresUtc { get; set; }
    
    /// <summary>
    /// Session'daki toplam tıklama sayısı
    /// </summary>
    public int ClickCount { get; set; }
    
    /// <summary>
    /// Session geçerli mi?
    /// </summary>
    public bool IsValid => DateTime.UtcNow < ExpiresUtc && DeletedUtc == null;
    
    /// <summary>
    /// Bu session'a ait tıklamalar
    /// </summary>
    public ICollection<ClickEvent> Clicks { get; set; } = new List<ClickEvent>();
}
