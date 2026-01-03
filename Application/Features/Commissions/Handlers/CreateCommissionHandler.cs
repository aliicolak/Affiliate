using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Features.Commissions.Commands;
using Domain.Entities.Commission;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commissions.Handlers;

/// <summary>
/// Komisyon oluşturma handler'ı
/// </summary>
public sealed class CreateCommissionHandler : IRequestHandler<CreateCommissionCommand, long>
{
    private readonly IAppDbContext _context;
    private readonly ICommissionCalculator _calculator;

    public CreateCommissionHandler(IAppDbContext context, ICommissionCalculator calculator)
    {
        _context = context;
        _calculator = calculator;
    }

    public async Task<long> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        // Click event'i al ve offer/program bilgilerini çek
        var clickEvent = await _context.ClickEvents
            .Include(c => c.Offer)
            .FirstOrDefaultAsync(c => c.Id == request.ClickEventId, cancellationToken);

        if (clickEvent is null)
            throw new InvalidOperationException($"Click event not found: {request.ClickEventId}");

        // Komisyon hesapla
        var programId = clickEvent.Offer.ProgramId ?? 0;
        var calculation = await _calculator.CalculateAsync(
            request.PublisherId,
            programId,
            request.SaleAmount,
            cancellationToken);

        var commission = new Commission
        {
            PublisherId = request.PublisherId,
            ClickEventId = request.ClickEventId,
            ConversionId = request.ConversionId,
            SaleAmount = request.SaleAmount,
            CommissionAmount = calculation.CommissionAmount,
            CommissionRate = calculation.CommissionRate,
            Currency = request.Currency,
            Status = CommissionStatus.Pending
        };

        _context.Commissions.Add(commission);
        
        // Click event'i converted olarak işaretle
        clickEvent.IsConverted = true;
        
        await _context.SaveChangesAsync(cancellationToken);

        return commission.Id;
    }
}
