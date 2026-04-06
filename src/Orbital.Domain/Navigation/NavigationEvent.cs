using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Domain.Navigation;

public sealed class NavigationEvent
{
    public Guid Id { get; private set; }
    public RingId RingId { get; private set; } = default!;
    public SiteId DestinationSiteId { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; }

    private NavigationEvent() { }

    public static NavigationEvent Record(RingId ringId, SiteId destinationSiteId) => new()
    {
        Id = Guid.NewGuid(),
        RingId = ringId,
        DestinationSiteId = destinationSiteId,
        OccurredAt = DateTime.UtcNow
    };
}
