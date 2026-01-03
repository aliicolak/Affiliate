using Application.Abstractions.Persistence;
using Application.Features.Commissions.DTOs;
using Application.Features.Commissions.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commissions.Handlers;

/// <summary>
/// Kazanç özeti handler'ı
/// </summary>
public sealed class GetPublisherEarningsSummaryHandler 
    : IRequestHandler<GetPublisherEarningsSummaryQuery, EarningsSummaryDto>
{
    private readonly IAppDbContext _context;

    public GetPublisherEarningsSummaryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EarningsSummaryDto> Handle(
        GetPublisherEarningsSummaryQuery request, 
        CancellationToken cancellationToken)
    {
        var baseQuery = _context.Commissions
            .AsNoTracking()
            .Where(c => c.PublisherId == request.PublisherId);

        var now = DateTime.UtcNow;
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);

        // Toplam kazanç (onaylanmış)
        var totalEarnings = await baseQuery
            .Where(c => c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Bekleyen
        var pendingEarnings = await baseQuery
            .Where(c => c.Status == CommissionStatus.Pending)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Ödenen
        var paidEarnings = await baseQuery
            .Where(c => c.Status == CommissionStatus.Paid)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Çekilebilir (Approved ama henüz Paid değil)
        var availableBalance = await baseQuery
            .Where(c => c.Status == CommissionStatus.Approved && c.PayoutId == null)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Son 7 gün
        var last7DaysEarnings = await baseQuery
            .Where(c => c.CreatedUtc >= last7Days && 
                       (c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid))
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Son 30 gün
        var last30DaysEarnings = await baseQuery
            .Where(c => c.CreatedUtc >= last30Days && 
                       (c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid))
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        // Komisyon sayıları
        var totalCommissions = await baseQuery.CountAsync(cancellationToken);
        var approvedCommissions = await baseQuery
            .CountAsync(c => c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid, cancellationToken);

        // Dönem kazancı (eğer tarih verilmişse)
        decimal periodEarnings = 0;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var periodQuery = baseQuery
                .Where(c => c.Status == CommissionStatus.Approved || c.Status == CommissionStatus.Paid);

            if (request.StartDate.HasValue)
                periodQuery = periodQuery.Where(c => c.CreatedUtc >= request.StartDate.Value);
            if (request.EndDate.HasValue)
                periodQuery = periodQuery.Where(c => c.CreatedUtc <= request.EndDate.Value);

            periodEarnings = await periodQuery.SumAsync(c => c.CommissionAmount, cancellationToken);
        }

        return new EarningsSummaryDto
        {
            TotalEarnings = totalEarnings,
            PendingEarnings = pendingEarnings,
            PaidEarnings = paidEarnings,
            AvailableBalance = availableBalance,
            PeriodEarnings = periodEarnings,
            Last7DaysEarnings = last7DaysEarnings,
            Last30DaysEarnings = last30DaysEarnings,
            TotalCommissions = totalCommissions,
            ApprovedCommissions = approvedCommissions
        };
    }
}
