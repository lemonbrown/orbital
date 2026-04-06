using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetRing;

public sealed record RingDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    RingVisibility Visibility,
    Guid OwnerUserId,
    DateTime CreatedAt,
    IReadOnlyList<MembershipDto> Memberships,
    IReadOnlyList<EdgeDto> Edges);

public sealed record MembershipDto(
    Guid Id,
    Guid SiteId,
    MembershipRole Role,
    MembershipStatus Status,
    int OrderIndex,
    DateTime JoinedAt);

public sealed record EdgeDto(
    Guid Id,
    Guid FromSiteId,
    Guid ToSiteId,
    string Dimension,
    string Label,
    int Weight);
