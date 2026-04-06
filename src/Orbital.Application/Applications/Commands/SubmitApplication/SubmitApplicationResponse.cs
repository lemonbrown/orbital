using Orbital.Domain.Rings;

namespace Orbital.Application.Applications.Commands.SubmitApplication;

public sealed record SubmitApplicationResponse(
    Guid MembershipId,
    Guid SiteId,
    MembershipStatus Status,
    string? SnippetHtml,
    string? CheckSnippetUrl);
