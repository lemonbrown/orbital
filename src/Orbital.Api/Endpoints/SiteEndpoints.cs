using MediatR;
using Microsoft.AspNetCore.Authorization;
using Orbital.Application.Sites.Commands.AddSite;
using Orbital.Application.Sites.Commands.VerifySite;
using Orbital.Application.Sites.Queries.GetMySites;

namespace Orbital.Api.Endpoints;

public static class SiteEndpoints
{
    public static RouteGroupBuilder MapSiteEndpoints(this RouteGroupBuilder group)
    {
        group.RequireAuthorization();

        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMySitesQuery(), ct);
            return Results.Ok(result);
        });

        group.MapPost("/", async (AddSiteCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/sites/{result.Id}", result);
        });

        group.MapPost("/{id:guid}/verify", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new VerifySiteCommand(id), ct);
            return Results.Ok(result);
        });

        return group;
    }
}
