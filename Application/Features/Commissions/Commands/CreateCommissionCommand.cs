using Application.Abstractions;
using Domain.Enums;

namespace Application.Features.Commissions.Commands;

/// <summary>
/// Yeni komisyon oluşturma (genellikle conversion sonrası otomatik çağrılır)
/// </summary>
public sealed record CreateCommissionCommand(
    long PublisherId,
    long ClickEventId,
    long? ConversionId,
    decimal SaleAmount,
    string Currency = "TRY"
) : ICommand<long>;
