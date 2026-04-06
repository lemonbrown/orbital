using MediatR;
using Orbital.Application.Navigation.Queries.Navigate;

namespace Orbital.Api.Endpoints;

public static class NavigationEndpoints
{
    public static RouteGroupBuilder MapNavigationEndpoints(this RouteGroupBuilder group)
    {
        // Redirect navigation endpoint — used as <a href="/api/navigate?ring=...&site=...&dimension=default&direction=next">
        group.MapGet("/", async (
            Guid ring,
            Guid site,
            string dimension,
            string direction,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new NavigateQuery(ring, site, dimension, direction), ct);
            return Results.Redirect(result.TargetSiteUrl);
        });

        return group;
    }
}
