using Application.Abstractions;

namespace Application.Features.Products.Commands;

public sealed record DeleteProductCommand(long Id) : ICommand<bool>;
