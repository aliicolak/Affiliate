using Application.Abstractions;
using Domain.Entities.Affiliate;

namespace Application.Features.Merchants.Commands
{
    public sealed record UpdateMerchantCommand(
        long Id,
        string Name,
        string? Website,
        bool IsActive
    ) : ICommand<Merchant?>;
}
