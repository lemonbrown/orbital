using Microsoft.EntityFrameworkCore;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Persistence.Repositories;

public sealed class RingRepository(OrbitalDbContext db) : IRingRepository
{
    public Task<Ring?> FindByIdAsync(RingId id, CancellationToken ct = default) =>
        db.Rings.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<Ring?> FindByIdWithDetailsAsync(RingId id, CancellationToken ct = default) =>
        db.Rings
            .Include(r => r.Memberships)
            .Include(r => r.Edges)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Ring>> GetPublicRingsAsync(CancellationToken ct = default) =>
        await db.Rings
            .Where(r => r.Visibility == RingVisibility.Public)
            .Include(r => r.Memberships)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Ring>> GetByOwnerAsync(UserId ownerId, CancellationToken ct = default) =>
        await db.Rings
            .Where(r => r.OwnerUserId == ownerId)
            .Include(r => r.Memberships)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Ring ring, CancellationToken ct = default) =>
        await db.Rings.AddAsync(ring, ct);
}
