using Application.Abstractions;
using Domain.Enums;

namespace Application.Features.Commissions.Commands;

/// <summary>
/// Komisyon durumunu güncelleme (Admin)
/// </summary>
public sealed record UpdateCommissionStatusCommand(
    long CommissionId,
    CommissionStatus NewStatus,
    string? Note = null,
    string? RejectionReason = null
) : ICommand<bool>;
