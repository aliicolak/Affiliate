using Application.Abstractions;

namespace Application.Features.Merchants.Commands
{
    public sealed record DeleteMerchantCommand(long Id) : ICommand<bool>;
}
