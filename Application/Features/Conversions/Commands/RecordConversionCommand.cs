using Application.Abstractions;

namespace Application.Features.Conversions.Commands;

/// <summary>
/// Webhook/Postback üzerinden conversion kaydetme
/// </summary>
public sealed record RecordConversionCommand(
    string TrackingCode,
    string ExternalOrderId,
    decimal SaleAmount,
    string Currency,
    string? ProductInfo,
    string? CustomerHash,
    string? RawPostback
) : ICommand<long>;
