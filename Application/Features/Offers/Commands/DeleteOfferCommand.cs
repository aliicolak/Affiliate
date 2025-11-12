using Application.Abstractions;

namespace Application.Features.Offers.Commands
{
    public sealed record DeleteOfferCommand(long Id) : ICommand<bool>;
}
