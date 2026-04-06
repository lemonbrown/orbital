using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Rings.Queries.GetRing;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Sites;

namespace Orbital.Application.Rings.Queries.GetRingBySlug;

public sealed class GetRingBySlugQueryHandler(IRingRepository ringRepository, ISiteRepository siteRepository)
    : IRequestHandler<GetRingBySlugQuery, RingDetailDto>
{
    public async Task<RingDetailDto> Handle(GetRingBySlugQuery request, CancellationToken cancellationToken)
    {
        var ring = await ringRepository.FindBySlugWithDetailsAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Ring with slug '{request.Slug}' was not found.");

        var siteIds = ring.Memberships.Select(m => m.SiteId).Distinct();
        var sites = await siteRepository.GetByIdsAsync(siteIds, cancellationToken);
        var siteMap = sites.ToDictionary(s => s.Id, s => s);

        var ac = ring.ActivityConfig;

        return new RingDetailDto(
            ring.Id.Value,
            ring.Name,
            ring.Slug,
            ring.Description,
            ring.Visibility,
            ring.VerificationMode,
            ring.ApprovalMode,
            ring.IsPublicJoinEnabled,
            ring.IsApiOnboardingEnabled,
            ring.IsPublicDirectoryEnabled,
            ring.OwnerUserId.Value,
            ring.CreatedAt,
            ring.Memberships.Select(m =>
            {
                siteMap.TryGetValue(m.SiteId, out var site);
                return new MembershipDto(
                    m.Id.Value, m.SiteId.Value,
                    site?.Name ?? m.SiteId.Value.ToString(),
                    site?.Url.Value ?? string.Empty,
                    m.Role, m.Status, m.OrderIndex, m.JoinedAt,
                    m.ApplicantName, m.ContactEmail);
            }).ToList(),
            ring.Edges.Select(e => new EdgeDto(
                e.Id.Value, e.FromSiteId.Value, e.ToSiteId.Value,
                e.Dimension.Value, e.Label.Value, e.Weight)).ToList(),
            new ActivityConfigDto(
                ac.IsEnabled,
                ac.CrawlingEnabled,
                ac.RecentPostWeight,
                ac.RecentUpdateWeight,
                ac.ActivityWindowDays,
                ac.CrawlFrequency.ToString(),
                ac.SkipStaleSites,
                ac.StaleSiteThresholdDays));
    }
}
