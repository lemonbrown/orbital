using Orbital.Domain.Sites;

namespace Orbital.Application.Sites.Commands.AddSite;

public sealed record AddSiteResponse(
    Guid Id,
    string Name,
    string Url,
    string Description,
    string VerificationToken,
    SiteVerificationStatus VerificationStatus);
