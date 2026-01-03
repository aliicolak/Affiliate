using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Features.Commissions.Commands;
using Application.Features.Conversions.Commands;
using Domain.Entities.Conversion;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversions.Handlers;

/// <summary>
/// Conversion kaydetme handler'ı
/// </summary>
public sealed class RecordConversionHandler : IRequestHandler<RecordConversionCommand, long>
{
    private readonly IAppDbContext _context;
    private readonly IMediator _mediator;

    public RecordConversionHandler(IAppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<long> Handle(RecordConversionCommand request, CancellationToken cancellationToken)
    {
        // Tracking code'dan click event'i bul
        var clickEvent = await _context.ClickEvents
            .Include(c => c.Offer)
            .FirstOrDefaultAsync(c => c.TrackingCode == request.TrackingCode, cancellationToken);

        if (clickEvent is null)
            throw new InvalidOperationException($"Click event not found for tracking code: {request.TrackingCode}");

        // Duplicate kontrolü
        var existingConversion = await _context.Conversions
            .AnyAsync(c => c.ExternalOrderId == request.ExternalOrderId, cancellationToken);

        if (existingConversion)
            throw new InvalidOperationException($"Conversion already exists for order: {request.ExternalOrderId}");

        // Conversion oluştur
        var conversion = new Conversion
        {
            ClickEventId = clickEvent.Id,
            ExternalOrderId = request.ExternalOrderId,
            SaleAmount = request.SaleAmount,
            Currency = request.Currency,
            ProductInfo = request.ProductInfo,
            CustomerHash = request.CustomerHash,
            RawPostback = request.RawPostback,
            Status = ConversionStatus.Pending
        };

        _context.Conversions.Add(conversion);
        await _context.SaveChangesAsync(cancellationToken);

        // Komisyon oluştur (publisher varsa)
        if (clickEvent.PublisherId.HasValue)
        {
            await _mediator.Send(new CreateCommissionCommand(
                clickEvent.PublisherId.Value,
                clickEvent.Id,
                conversion.Id,
                request.SaleAmount,
                request.Currency), cancellationToken);
        }

        return conversion.Id;
    }
}
