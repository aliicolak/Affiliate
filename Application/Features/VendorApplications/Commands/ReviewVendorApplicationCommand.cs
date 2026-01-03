using Application.Abstractions;
using Domain.Enums;

namespace Application.Features.VendorApplications.Commands;

/// <summary>
/// Başvuru inceleme (Admin)
/// </summary>
public sealed record ReviewVendorApplicationCommand(
    long ApplicationId,
    ApplicationStatus NewStatus,
    long ReviewerId,
    string? AdminNote,
    string? RejectionReason
) : ICommand<long?>; // Onay durumunda oluşturulan Merchant ID döner
