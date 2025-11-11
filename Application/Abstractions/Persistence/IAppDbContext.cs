using Domain.Entities.Affiliate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Application.Abstractions.Persistence;

public interface IAppDbContext
{
    DbSet<Merchant> Merchants { get; }
    DbSet<AffiliateProgram> AffiliatePrograms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
