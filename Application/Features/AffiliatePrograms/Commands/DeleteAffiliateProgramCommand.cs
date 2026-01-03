using Application.Abstractions;

namespace Application.Features.AffiliatePrograms.Commands;

public sealed record DeleteAffiliateProgramCommand(long Id) : ICommand<bool>;
