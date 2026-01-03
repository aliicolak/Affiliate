using Application.Abstractions.Persistence;
using Application.Features.Brands.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Brands.Handlers;

public sealed class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteBrandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (brand is null) return false;

        brand.DeletedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
