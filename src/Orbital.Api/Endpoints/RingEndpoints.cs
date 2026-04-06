using MediatR;
using Orbital.Application.Rings.Commands.ApproveMember;
using Orbital.Application.Rings.Commands.CheckSnippet;
using Orbital.Application.Rings.Commands.CreateApiKey;
using Orbital.Application.Rings.Commands.CreateRing;
using Orbital.Application.Rings.Commands.JoinRing;
using Orbital.Application.Rings.Commands.DeleteRing;
using Orbital.Application.Rings.Commands.RejectMember;
using Orbital.Application.Rings.Commands.RemoveMember;
using Orbital.Application.Rings.Commands.RevokeApiKey;
using Orbital.Application.Rings.Commands.UpdateActivityConfig;
using Orbital.Application.Rings.Commands.UpdateRingSettings;
using Orbital.Application.Rings.Queries.GetApiKeys;
using Orbital.Application.Rings.Queries.GetMyRings;
using Orbital.Application.Rings.Queries.GetPublicRings;
using Orbital.Application.Rings.Queries.GetRing;
using Orbital.Application.Rings.Queries.GetRingBySlug;
using Orbital.Application.Rings.Queries.GetRingDirectory;
using Orbital.Domain.Rings;

namespace Orbital.Api.Endpoints;

public static class RingEndpoints
{
    public static RouteGroupBuilder MapRingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/public", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPublicRingsQuery(), ct);
            return Results.Ok(result);
        });

        group.MapGet("/mine", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyRingsQuery(), ct);
            return Results.Ok(result);
        }).RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRingQuery(id), ct);
            return Results.Ok(result);
        });

        group.MapGet("/slug/{slug}", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRingBySlugQuery(slug), ct);
            return Results.Ok(result);
        });

        group.MapGet("/slug/{slug}/directory", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRingDirectoryQuery(slug), ct);
            return Results.Ok(result);
        });

        group.MapPost("/", async (CreateRingCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/rings/{result.Id}", result);
        }).RequireAuthorization();

        group.MapPost("/{ringId:guid}/join", async (Guid ringId, JoinRingRequest req, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new JoinRingCommand(ringId, req.SiteId), ct);
            return Results.Ok(result);
        }).RequireAuthorization();

        group.MapPost("/{ringId:guid}/memberships/{membershipId:guid}/approve",
            async (Guid ringId, Guid membershipId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ApproveMemberCommand(ringId, membershipId), ct);
                return Results.Ok(result);
            }).RequireAuthorization();

        group.MapPost("/{ringId:guid}/memberships/{membershipId:guid}/reject",
            async (Guid ringId, Guid membershipId, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new RejectMemberCommand(ringId, membershipId), ct);
                return Results.NoContent();
            }).RequireAuthorization();

        group.MapPost("/{ringId:guid}/memberships/{membershipId:guid}/check-snippet",
            async (Guid ringId, Guid membershipId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CheckSnippetCommand(ringId, membershipId), ct);
                return Results.Ok(result);
            });

        group.MapPatch("/{ringId:guid}/settings",
            async (Guid ringId, UpdateSettingsRequest req, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new UpdateRingSettingsCommand(
                    ringId,
                    req.IsPublicJoinEnabled,
                    req.IsApiOnboardingEnabled,
                    req.IsPublicDirectoryEnabled,
                    req.VerificationMode,
                    req.ApprovalMode), ct);
                return Results.NoContent();
            }).RequireAuthorization();

        group.MapGet("/{ringId:guid}/api-keys",
            async (Guid ringId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetApiKeysQuery(ringId), ct);
                return Results.Ok(result);
            }).RequireAuthorization();

        group.MapPost("/{ringId:guid}/api-keys",
            async (Guid ringId, CreateApiKeyRequest req, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new CreateApiKeyCommand(ringId, req.Label), ct);
                return Results.Created($"/api/rings/{ringId}/api-keys/{result.KeyId}", result);
            }).RequireAuthorization();

        group.MapDelete("/{ringId:guid}/api-keys/{keyId:guid}",
            async (Guid ringId, Guid keyId, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new RevokeApiKeyCommand(ringId, keyId), ct);
                return Results.NoContent();
            }).RequireAuthorization();

        group.MapDelete("/{ringId:guid}/memberships/{membershipId:guid}",
            async (Guid ringId, Guid membershipId, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new RemoveMemberCommand(ringId, membershipId), ct);
                return Results.NoContent();
            }).RequireAuthorization();

        group.MapDelete("/{ringId:guid}",
            async (Guid ringId, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new DeleteRingCommand(ringId), ct);
                return Results.NoContent();
            }).RequireAuthorization();

        group.MapPatch("/{ringId:guid}/activity-config",
            async (Guid ringId, UpdateActivityConfigRequest req, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new UpdateActivityConfigCommand(
                    ringId,
                    req.IsEnabled,
                    req.CrawlingEnabled,
                    req.RecentPostWeight,
                    req.RecentUpdateWeight,
                    req.ActivityWindowDays,
                    req.CrawlFrequency,
                    req.SkipStaleSites,
                    req.StaleSiteThresholdDays), ct);
                return Results.NoContent();
            }).RequireAuthorization();

        return group;
    }

    private sealed record JoinRingRequest(Guid SiteId);
    private sealed record UpdateSettingsRequest(bool IsPublicJoinEnabled, bool IsApiOnboardingEnabled, bool IsPublicDirectoryEnabled, VerificationMode VerificationMode, ApprovalMode ApprovalMode);
    private sealed record CreateApiKeyRequest(string Label);
    private sealed record UpdateActivityConfigRequest(bool IsEnabled, bool CrawlingEnabled, decimal RecentPostWeight, decimal RecentUpdateWeight, int ActivityWindowDays, string CrawlFrequency, bool SkipStaleSites, int StaleSiteThresholdDays);
}
