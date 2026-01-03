using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Commission;

/// <summary>
/// Ödeme talebi
/// Publisher'lar biriken komisyonlarını çekmek için talep oluşturur
/// </summary>
public class Payout : BaseEntity
{
    /// <summary>
    /// Ödeme talebinde bulunan publisher
    /// </summary>
    public long PublisherId { get; set; }
    
    /// <summary>
    /// Talep edilen tutar
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Para birimi
    /// </summary>
    public string Currency { get; set; } = "TRY";
    
    /// <summary>
    /// Ödeme durumu
    /// </summary>
    public PayoutStatus Status { get; set; } = PayoutStatus.Requested;
    
    /// <summary>
    /// Ödeme yöntemi (Bank, PayPal, Crypto vb.)
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;
    
    /// <summary>
    /// Ödeme detayları (JSON - banka bilgileri vb.)
    /// </summary>
    public string? PaymentDetails { get; set; }
    
    /// <summary>
    /// İşlem ücreti
    /// </summary>
    public decimal Fee { get; set; }
    
    /// <summary>
    /// Net ödeme tutarı
    /// </summary>
    public decimal NetAmount => Amount - Fee;
    
    /// <summary>
    /// İşlem referans numarası
    /// </summary>
    public string? TransactionRef { get; set; }
    
    /// <summary>
    /// İşlem zamanı
    /// </summary>
    public DateTime? ProcessedUtc { get; set; }
    
    /// <summary>
    /// Admin notu
    /// </summary>
    public string? AdminNote { get; set; }
    
    /// <summary>
    /// Red sebebi
    /// </summary>
    public string? RejectionReason { get; set; }
    
    // Navigation
    public ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
