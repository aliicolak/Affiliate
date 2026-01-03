using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Komisyon hesaplama servisi
/// Tier-based hesaplama yapar
/// </summary>
public sealed class CommissionCalculator : ICommissionCalculator
{
    private readonly IAppDbContext _context;

    public CommissionCalculator(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CommissionCalculation> CalculateAsync(
        long publisherId,
        long affiliateProgramId,
        decimal saleAmount,
        CancellationToken cancellationToken = default)
    {
        // Affiliate program'ı al
        var program = await _context.AffiliatePrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == affiliateProgramId, cancellationToken);

        if (program is null)
            throw new InvalidOperationException($"Affiliate program not found: {affiliateProgramId}");

        // Publisher'ın bu program'daki tier'ını bul
        var tier = await GetPublisherTierAsync(publisherId, affiliateProgramId, cancellationToken);

        if (tier is not null)
        {
            // Tier bazlı hesaplama
            if (tier.FixedAmount.HasValue)
            {
                return new CommissionCalculation(
                    tier.FixedAmount.Value,
                    0,
                    tier.Name,
                    IsFixedAmount: true);
            }

            var commissionAmount = saleAmount * (tier.CommissionRate / 100);
            return new CommissionCalculation(
                Math.Round(commissionAmount, 2),
                tier.CommissionRate,
                tier.Name,
                IsFixedAmount: false);
        }

        // Default: Program'ın base commission'ını kullan
        var baseCommission = saleAmount * (program.BaseCommissionPct / 100);
        return new CommissionCalculation(
            Math.Round(baseCommission, 2),
            program.BaseCommissionPct,
            "Base",
            IsFixedAmount: false);
    }

    private async Task<Domain.Entities.Commission.CommissionTier?> GetPublisherTierAsync(
        long publisherId,
        long affiliateProgramId,
        CancellationToken cancellationToken)
    {
        // Publisher'ın bu program'daki toplam satış sayısını hesapla
        var salesCount = await _context.Commissions
            .Where(c => c.PublisherId == publisherId &&
                        c.ClickEvent.Offer.ProgramId == affiliateProgramId &&
                        c.Status == Domain.Enums.CommissionStatus.Approved)
            .CountAsync(cancellationToken);

        var salesAmount = await _context.Commissions
            .Where(c => c.PublisherId == publisherId &&
                        c.ClickEvent.Offer.ProgramId == affiliateProgramId &&
                        c.Status == Domain.Enums.CommissionStatus.Approved)
            .SumAsync(c => c.SaleAmount, cancellationToken);

        // En yüksek uyan tier'ı bul
        var tier = await _context.CommissionTiers
            .Where(t => t.AffiliateProgramId == affiliateProgramId &&
                        t.IsActive &&
                        t.MinSalesCount <= salesCount &&
                        t.MinSalesAmount <= salesAmount)
            .OrderByDescending(t => t.SortOrder)
            .FirstOrDefaultAsync(cancellationToken);

        return tier;
    }
}
