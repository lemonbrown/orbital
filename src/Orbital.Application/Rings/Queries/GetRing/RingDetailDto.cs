using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetRing;

public sealed record RingDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    RingVisibility Visibility,
    VerificationMode VerificationMode,
    ApprovalMode ApprovalMode,
    bool IsPublicJoinEnabled,
    bool IsApiOnboardingEnabled,
    bool IsPublicDirectoryEnabled,
    Guid OwnerUserId,
    DateTime CreatedAt,
    IReadOnlyList<MembershipDto> Memberships,
    IReadOnlyList<EdgeDto> Edges,
    ActivityConfigDto ActivityConfig);

public sealed record ActivityConfigDto(
    bool IsEnabled,
    bool CrawlingEnabled,
    decimal RecentPostWeight,
    decimal RecentUpdateWeight,
    int ActivityWindowDays,
    string CrawlFrequency,
    bool SkipStaleSites,
    int StaleSiteThresholdDays);
public sealed record MembershipDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    string SiteUrl,
    MembershipRole Role,
    MembershipStatus Status,
    int OrderIndex,
    DateTime JoinedAt,
    string? ApplicantName,
    string? ContactEmail);

public sealed record EdgeDto(
    Guid Id,
    Guid FromSiteId,
    Guid ToSiteId,
    string Dimension,
    string Label,
    int Weight);
