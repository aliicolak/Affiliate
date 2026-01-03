using Application.Abstractions.Persistence;
using Application.Features.Commissions.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commissions.Handlers;

/// <summary>
/// Komisyon durumu güncelleme handler'ı
/// </summary>
public sealed class UpdateCommissionStatusHandler : IRequestHandler<UpdateCommissionStatusCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateCommissionStatusHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCommissionStatusCommand request, CancellationToken cancellationToken)
    {
        var commission = await _context.Commissions
            .FirstOrDefaultAsync(c => c.Id == request.CommissionId, cancellationToken);

        if (commission is null) return false;

        commission.Status = request.NewStatus;
        commission.StatusChangedUtc = DateTime.UtcNow;
        commission.MerchantNote = request.Note;

        if (request.NewStatus == Domain.Enums.CommissionStatus.Rejected)
        {
            commission.RejectionReason = request.RejectionReason;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
