using Application.Abstractions.Persistence;
using Application.Features.Payouts.Commands;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payouts.Handlers;

/// <summary>
/// Ödeme işleme handler'ı (Admin)
/// </summary>
public sealed class ProcessPayoutHandler : IRequestHandler<ProcessPayoutCommand, bool>
{
    private readonly IAppDbContext _context;

    public ProcessPayoutHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ProcessPayoutCommand request, CancellationToken cancellationToken)
    {
        var payout = await _context.Payouts
            .Include(p => p.Commissions)
            .FirstOrDefaultAsync(p => p.Id == request.PayoutId, cancellationToken);

        if (payout is null) return false;

        payout.Status = request.NewStatus;
        payout.AdminNote = request.AdminNote;

        if (request.NewStatus == PayoutStatus.Completed)
        {
            payout.ProcessedUtc = DateTime.UtcNow;
            payout.TransactionRef = request.TransactionRef;

            // Komisyonları Paid olarak işaretle
            foreach (var commission in payout.Commissions)
            {
                commission.Status = CommissionStatus.Paid;
                commission.StatusChangedUtc = DateTime.UtcNow;
            }
        }
        else if (request.NewStatus == PayoutStatus.Rejected || request.NewStatus == PayoutStatus.Failed)
        {
            payout.RejectionReason = request.RejectionReason;

            // Komisyonları tekrar available yap
            foreach (var commission in payout.Commissions)
            {
                commission.PayoutId = null;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
