using Orbital.Domain.Common;
using Orbital.Domain.Sites;

namespace Orbital.Domain.Rings;

public sealed class Edge : Entity<EdgeId>
{
    public RingId RingId { get; private set; }
    public SiteId FromSiteId { get; private set; }
    public SiteId ToSiteId { get; private set; }
    public Dimension Dimension { get; private set; } = default!;
    public EdgeLabel Label { get; private set; } = default!;
    public int Weight { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Edge() { }

    internal static Edge Create(
        RingId ringId,
        SiteId fromSiteId,
        SiteId toSiteId,
        Dimension dimension,
        EdgeLabel label,
        int weight = 0) =>
        new()
        {
            Id = EdgeId.New(),
            RingId = ringId,
            FromSiteId = fromSiteId,
            ToSiteId = toSiteId,
            Dimension = dimension,
            Label = label,
            Weight = weight,
            CreatedAt = DateTime.UtcNow
        };
}
