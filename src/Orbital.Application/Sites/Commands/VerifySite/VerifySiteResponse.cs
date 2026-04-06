using Orbital.Domain.Sites;

namespace Orbital.Application.Sites.Commands.VerifySite;

public sealed record VerifySiteResponse(bool Success, SiteVerificationStatus Status, string? Message);
