using Orbital.Domain.Navigation;
using Orbital.Domain.Rings;

namespace Orbital.Domain.Interfaces;

public interface INavigationEventRepository
{
    Task AddAsync(NavigationEvent @event, CancellationToken ct = default);
    Task<HashSet<Guid>> GetActiveSiteIdsAsync(RingId ringId, DateTime since, CancellationToken ct = default);
}
