using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Navigation;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Application.Navigation.Queries.Navigate;

public sealed class NavigateQueryHandler(
    IRingRepository ringRepository,
    ISiteRepository siteRepository,
    INavigationEventRepository navigationEventRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<NavigateQuery, NavigateResult>
{
    public async Task<NavigateResult> Handle(NavigateQuery request, CancellationToken cancellationToken)
    {
        var ringId = RingId.From(request.RingId);
        var ring = await ringRepository.FindByIdWithDetailsAsync(ringId, cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var siteId = SiteId.From(request.SiteId);
        var dimension = Dimension.FromString(request.Dimension);
        var label = request.Direction.ToLowerInvariant() == "prev"
            ? EdgeLabel.Previous
            : EdgeLabel.Next;

        var firstEdge = ring.Edges
            .Where(e => e.FromSiteId == siteId
                        && e.Dimension == dimension
                        && e.Label == label)
            .OrderBy(e => e.Weight)
            .FirstOrDefault()
            ?? throw new NotFoundException($"No '{request.Direction}' edge found in ring for site '{request.SiteId}' on dimension '{request.Dimension}'.");

        var targetSiteId = firstEdge.ToSiteId;

        if (ring.ActivityConfig.SkipStaleSites)
        {
            var since = DateTime.UtcNow.AddDays(-ring.ActivityConfig.StaleSiteThresholdDays);
            var activeSiteIds = await navigationEventRepository.GetActiveSiteIdsAsync(ringId, since, cancellationToken);

            if (!activeSiteIds.Contains(targetSiteId.Value))
            {
                // Walk the ring until we find an active site or come full circle
                var visited = new HashSet<Guid> { siteId.Value, targetSiteId.Value };
                var current = targetSiteId;

                while (true)
                {
                    var nextEdge = ring.Edges
                        .Where(e => e.FromSiteId == current
                                    && e.Dimension == dimension
                                    && e.Label == label)
                        .OrderBy(e => e.Weight)
                        .FirstOrDefault();

                    if (nextEdge == null || visited.Contains(nextEdge.ToSiteId.Value))
                        break; // full circle — fall back to direct next

                    if (activeSiteIds.Contains(nextEdge.ToSiteId.Value))
                    {
                        targetSiteId = nextEdge.ToSiteId;
                        break;
                    }

                    visited.Add(nextEdge.ToSiteId.Value);
                    current = nextEdge.ToSiteId;
                }
            }
        }

        var targetSite = await siteRepository.FindByIdAsync(targetSiteId, cancellationToken)
            ?? throw NotFoundException.For("Site", targetSiteId);

        await navigationEventRepository.AddAsync(
            NavigationEvent.Record(ringId, targetSiteId), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new NavigateResult(
            targetSite.Id.Value,
            targetSite.Url.Value,
            targetSite.Name,
            request.Dimension,
            request.Direction);
    }
}
