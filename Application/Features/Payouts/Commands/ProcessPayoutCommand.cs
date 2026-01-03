using Application.Abstractions;
using Domain.Enums;

namespace Application.Features.Payouts.Commands;

/// <summary>
/// Ödeme talebi durumu güncelleme (Admin)
/// </summary>
public sealed record ProcessPayoutCommand(
    long PayoutId,
    PayoutStatus NewStatus,
    string? TransactionRef = null,
    string? AdminNote = null,
    string? RejectionReason = null
) : ICommand<bool>;
