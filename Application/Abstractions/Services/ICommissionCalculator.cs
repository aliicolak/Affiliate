namespace Application.Abstractions.Services;

/// <summary>
/// Komisyon hesaplama servisi
/// Open/Closed: Farklı hesaplama stratejileri extend edilebilir
/// </summary>
public interface ICommissionCalculator
{
    /// <summary>
    /// Verilen satış için komisyon hesaplar
    /// </summary>
    Task<CommissionCalculation> CalculateAsync(
        long publisherId,
        long affiliateProgramId,
        decimal saleAmount,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Komisyon hesaplama sonucu
/// </summary>
public sealed record CommissionCalculation(
    decimal CommissionAmount,
    decimal CommissionRate,
    string TierName,
    bool IsFixedAmount
);
