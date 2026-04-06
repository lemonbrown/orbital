using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetRingDirectory;

public sealed class GetRingDirectoryQueryHandler(
    IRingRepository ringRepository,
    ISiteRepository siteRepository)
    : IRequestHandler<GetRingDirectoryQuery, IReadOnlyList<DirectorySiteDto>>
{
    public async Task<IReadOnlyList<DirectorySiteDto>> Handle(GetRingDirectoryQuery request, CancellationToken cancellationToken)
    {
        var ring = await ringRepository.FindBySlugWithDetailsAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Ring with slug '{request.Slug}' was not found.");

        if (!ring.IsPublicDirectoryEnabled)
        {
            throw new UnauthorizedException("Public directory is not enabled for this ring.");
        }

        var approvedMemberships = ring.Memberships
            .Where(m => m.Status == MembershipStatus.Approved)
            .OrderBy(m => m.OrderIndex)
            .ToList();

        if (approvedMemberships.Count == 0)
        {
            return Array.Empty<DirectorySiteDto>();
        }

        var siteIds = approvedMemberships.Select(m => m.SiteId).Distinct().ToList();
        var sites = await siteRepository.GetByIdsAsync(siteIds, cancellationToken);
        var sitesById = sites.ToDictionary(s => s.Id);

        var dtos = new List<DirectorySiteDto>();

        foreach (var m in approvedMemberships)
        {
            if (sitesById.TryGetValue(m.SiteId, out var site))
            {
                dtos.Add(new DirectorySiteDto(
                    site.Id.Value,
                    site.Name,
                    site.Url.Value,
                    site.Description,
                    m.OrderIndex));
            }
        }

        return dtos;
    }
}
