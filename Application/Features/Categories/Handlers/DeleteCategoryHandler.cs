using Application.Abstractions.Persistence;
using Application.Features.Categories.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Handlers;

public sealed class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteCategoryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null) return false;

        category.DeletedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
