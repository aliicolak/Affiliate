using Application.Abstractions.Persistence;
using Application.Features.Products.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers;

public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteProductHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .IgnoreQueryFilters() // soft delete filter'ı atla
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null) return false;

        // Soft delete
        product.DeletedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
