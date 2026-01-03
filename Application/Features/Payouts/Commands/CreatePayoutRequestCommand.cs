using Application.Abstractions;

namespace Application.Features.Payouts.Commands;

/// <summary>
/// Ödeme talebi oluşturma
/// </summary>
public sealed record CreatePayoutRequestCommand(
    long PublisherId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string? PaymentDetails
) : ICommand<long>;
