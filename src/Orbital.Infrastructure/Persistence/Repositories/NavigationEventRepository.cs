using Microsoft.EntityFrameworkCore;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Navigation;
using Orbital.Domain.Rings;

namespace Orbital.Infrastructure.Persistence.Repositories;

public sealed class NavigationEventRepository(OrbitalDbContext db) : INavigationEventRepository
{
    public async Task AddAsync(NavigationEvent @event, CancellationToken ct = default)
    {
        await db.NavigationEvents.AddAsync(@event, ct);
    }

    public async Task<HashSet<Guid>> GetActiveSiteIdsAsync(RingId ringId, DateTime since, CancellationToken ct = default)
    {
        var ids = await db.NavigationEvents
            .Where(e => e.RingId == ringId && e.OccurredAt >= since)
            .Select(e => e.DestinationSiteId.Value)
            .Distinct()
            .ToListAsync(ct);
        return ids.ToHashSet();
    }
}
