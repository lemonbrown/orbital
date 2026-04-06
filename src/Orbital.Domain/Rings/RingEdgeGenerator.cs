using Orbital.Domain.Sites;

namespace Orbital.Domain.Rings;

public static class RingEdgeGenerator
{
    /// <summary>
    /// Produces a symmetric circular edge list (next + previous) from an ordered membership list.
    /// </summary>
    public static IReadOnlyList<Edge> GenerateCircularEdges(
        RingId ringId,
        IReadOnlyList<SiteId> orderedSiteIds,
        Dimension dimension)
    {
        if (orderedSiteIds.Count < 2)
            return [];

        var edges = new List<Edge>(orderedSiteIds.Count * 2);
        var count = orderedSiteIds.Count;

        for (var i = 0; i < count; i++)
        {
            var current = orderedSiteIds[i];
            var next = orderedSiteIds[(i + 1) % count];
            var previous = orderedSiteIds[(i - 1 + count) % count];

            edges.Add(Edge.Create(ringId, current, next, dimension, EdgeLabel.Next, weight: i));
            edges.Add(Edge.Create(ringId, current, previous, dimension, EdgeLabel.Previous, weight: i));
        }

        return edges;
    }
}
