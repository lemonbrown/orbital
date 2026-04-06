using MediatR;
using Orbital.Application.Rings.Commands.ApproveMember;
using Orbital.Application.Rings.Commands.CreateRing;
using Orbital.Application.Rings.Commands.JoinRing;
using Orbital.Application.Rings.Commands.RejectMember;
using Orbital.Application.Rings.Queries.GetMyRings;
using Orbital.Application.Rings.Queries.GetPublicRings;
using Orbital.Application.Rings.Queries.GetRing;

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

        return group;
    }

    private sealed record JoinRingRequest(Guid SiteId);
}
