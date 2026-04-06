using MediatR;
using Orbital.Api.Services;
using Orbital.Application.Applications.Commands.SubmitApplication;
using Orbital.Application.Common.Interfaces;

namespace Orbital.Api.Endpoints;

public static class ApplicationEndpoints
{
    public static RouteGroupBuilder MapApplicationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            SubmitApplicationRequest req,
            ISender sender,
            ICurrentUserService currentUserService,
            RingApiKeyAuthService apiKeyAuth,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            // Resolve API key if provided
            var rawKey = httpContext.Request.Headers["X-Ring-Api-Key"].FirstOrDefault();
            var apiKeyRingId = await apiKeyAuth.ValidateAsync(rawKey, ct);
            var isApiKeyRequest = apiKeyRingId.HasValue;

            // Ring ID: from body, or inferred from API key
            var ringId = apiKeyRingId.HasValue ? apiKeyRingId.Value.Value : req.RingId;

            var command = new SubmitApplicationCommand(
                RingId: ringId,
                SiteUrl: req.SiteUrl,
                SiteName: req.SiteName,
                Description: req.Description,
                ContactEmail: req.ContactEmail,
                ApplicantName: req.ApplicantName,
                CallerUserId: currentUserService.IsAuthenticated ? currentUserService.UserId!.Value.Value : null,
                IsApiKeyRequest: isApiKeyRequest);

            var result = await sender.Send(command, ct);
            return Results.Ok(result);
        });

        return group;
    }

    private sealed record SubmitApplicationRequest(
        Guid RingId,
        string SiteUrl,
        string SiteName,
        string? Description,
        string? ContactEmail,
        string? ApplicantName);
}
