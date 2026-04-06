using Microsoft.EntityFrameworkCore;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Infrastructure.Persistence.Repositories;

public sealed class RingApiKeyRepository(OrbitalDbContext db) : IRingApiKeyRepository
{
    public Task<RingApiKey?> FindByPrefixAsync(string prefix, CancellationToken ct = default) =>
        db.RingApiKeys
            .Where(k => k.KeyPrefix == prefix && k.RevokedAt == null)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<RingApiKey>> GetByRingAsync(RingId ringId, CancellationToken ct = default) =>
        await db.RingApiKeys
            .Where(k => k.RingId == ringId)
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync(ct);
}
