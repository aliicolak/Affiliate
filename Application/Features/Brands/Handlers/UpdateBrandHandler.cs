using Application.Abstractions.Persistence;
using Application.Features.Brands.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Brands.Handlers;

public sealed class UpdateBrandHandler : IRequestHandler<UpdateBrandCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateBrandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (brand is null) return false;

        brand.Name = request.Name;
        brand.Slug = request.Slug.ToLower().Replace(" ", "-");
        brand.LogoAssetId = request.LogoAssetId;
        brand.IsActive = request.IsActive;
        brand.UpdatedUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
