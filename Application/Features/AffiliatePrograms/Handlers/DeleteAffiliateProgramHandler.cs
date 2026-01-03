using Application.Abstractions.Persistence;
using Application.Features.AffiliatePrograms.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AffiliatePrograms.Handlers;

public sealed class DeleteAffiliateProgramHandler : IRequestHandler<DeleteAffiliateProgramCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteAffiliateProgramHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteAffiliateProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _context.AffiliatePrograms
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (program is null) return false;

        program.DeletedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
