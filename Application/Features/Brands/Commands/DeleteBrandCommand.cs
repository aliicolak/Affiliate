using Application.Abstractions;

namespace Application.Features.Brands.Commands;

public sealed record DeleteBrandCommand(long Id) : ICommand<bool>;
