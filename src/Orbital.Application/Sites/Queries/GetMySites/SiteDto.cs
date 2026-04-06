using Orbital.Domain.Sites;

namespace Orbital.Application.Sites.Queries.GetMySites;

public sealed record SiteDto(
    Guid Id,
    string Name,
    string Url,
    string Description,
    SiteVerificationStatus VerificationStatus,
    string VerificationToken,
    DateTime CreatedAt);
