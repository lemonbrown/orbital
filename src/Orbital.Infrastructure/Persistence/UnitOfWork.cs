using Orbital.Domain.Interfaces;

namespace Orbital.Infrastructure.Persistence;

public sealed class UnitOfWork(OrbitalDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
