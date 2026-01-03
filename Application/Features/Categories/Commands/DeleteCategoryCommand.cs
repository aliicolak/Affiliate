using Application.Abstractions;

namespace Application.Features.Categories.Commands;

public sealed record DeleteCategoryCommand(long Id) : ICommand<bool>;
