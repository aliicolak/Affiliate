using Application.Abstractions.Persistence;
using Application.Features.Payouts.Commands;
using Domain.Entities.Commission;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payouts.Handlers;

/// <summary>
/// Ödeme talebi oluşturma handler'ı
/// </summary>
public sealed class CreatePayoutRequestHandler : IRequestHandler<CreatePayoutRequestCommand, long>
{
    private readonly IAppDbContext _context;
    private const decimal MinPayoutAmount = 100; // Minimum çekim tutarı

    public CreatePayoutRequestHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreatePayoutRequestCommand request, CancellationToken cancellationToken)
    {
        // Çekilebilir bakiye kontrolü
        var availableBalance = await _context.Commissions
            .Where(c => c.PublisherId == request.PublisherId &&
                        c.Status == CommissionStatus.Approved &&
                        c.PayoutId == null)
            .SumAsync(c => c.CommissionAmount, cancellationToken);

        if (request.Amount > availableBalance)
            throw new InvalidOperationException($"Insufficient balance. Available: {availableBalance}");

        if (request.Amount < MinPayoutAmount)
            throw new InvalidOperationException($"Minimum payout amount is {MinPayoutAmount}");

        // Bekleyen talep var mı kontrolü
        var hasPendingPayout = await _context.Payouts
            .AnyAsync(p => p.PublisherId == request.PublisherId &&
                          (p.Status == PayoutStatus.Requested || p.Status == PayoutStatus.Processing),
                      cancellationToken);

        if (hasPendingPayout)
            throw new InvalidOperationException("You already have a pending payout request");

        // Payout oluştur
        var payout = new Payout
        {
            PublisherId = request.PublisherId,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            PaymentDetails = request.PaymentDetails,
            Status = PayoutStatus.Requested,
            Fee = CalculateFee(request.Amount, request.PaymentMethod)
        };

        _context.Payouts.Add(payout);
        await _context.SaveChangesAsync(cancellationToken);

        // İlgili komisyonları payout'a bağla (FIFO)
        var commissions = await _context.Commissions
            .Where(c => c.PublisherId == request.PublisherId &&
                        c.Status == CommissionStatus.Approved &&
                        c.PayoutId == null)
            .OrderBy(c => c.CreatedUtc)
            .ToListAsync(cancellationToken);

        decimal allocated = 0;
        foreach (var commission in commissions)
        {
            if (allocated >= request.Amount) break;
            
            commission.PayoutId = payout.Id;
            allocated += commission.CommissionAmount;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return payout.Id;
    }

    private static decimal CalculateFee(decimal amount, string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "bank" => 0,
            "paypal" => amount * 0.02m, // %2 PayPal fee
            "crypto" => 5, // Sabit 5 TL network fee
            _ => 0
        };
    }
}
