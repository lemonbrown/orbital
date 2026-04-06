using MediatR;

namespace Orbital.Application.Applications.Commands.SubmitApplication;

public sealed record SubmitApplicationCommand(
    Guid RingId,
    string SiteUrl,
    string SiteName,
    string? Description,
    string? ContactEmail,
    string? ApplicantName,
    /// <summary>Set when submitted by an authenticated Smallorbit user.</summary>
    Guid? CallerUserId,
    /// <summary>True when request came through a valid ring API key (skips IsPublicJoinEnabled check).</summary>
    bool IsApiKeyRequest) : IRequest<SubmitApplicationResponse>;
