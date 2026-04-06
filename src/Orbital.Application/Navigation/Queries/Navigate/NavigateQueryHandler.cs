using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Application.Navigation.Queries.Navigate;

public sealed class NavigateQueryHandler(
    IRingRepository ringRepository,
    ISiteRepository siteRepository)
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

        var edge = ring.Edges
            .Where(e => e.FromSiteId == siteId
                        && e.Dimension == dimension
                        && e.Label == label)
            .OrderBy(e => e.Weight)
            .FirstOrDefault()
            ?? throw new NotFoundException($"No '{request.Direction}' edge found in ring for site '{request.SiteId}' on dimension '{request.Dimension}'.");

        var targetSite = await siteRepository.FindByIdAsync(edge.ToSiteId, cancellationToken)
            ?? throw NotFoundException.For("Site", edge.ToSiteId);

        return new NavigateResult(
            targetSite.Id.Value,
            targetSite.Url.Value,
            targetSite.Name,
            request.Dimension,
            request.Direction);
    }
}
