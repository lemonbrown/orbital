using Microsoft.EntityFrameworkCore;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Persistence.Repositories;

public sealed class SiteRepository(OrbitalDbContext db) : ISiteRepository
{
    public Task<Site?> FindByIdAsync(SiteId id, CancellationToken ct = default) =>
        db.Sites.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Site>> GetByOwnerAsync(UserId ownerId, CancellationToken ct = default) =>
        await db.Sites.Where(s => s.OwnerUserId == ownerId).ToListAsync(ct);

    public async Task AddAsync(Site site, CancellationToken ct = default) =>
        await db.Sites.AddAsync(site, ct);
}
