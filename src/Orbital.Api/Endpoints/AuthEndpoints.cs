using MediatR;
using Orbital.Application.Auth.Commands.LoginUser;
using Orbital.Application.Auth.Commands.RegisterUser;

namespace Orbital.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (RegisterUserCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/users/{result.UserId}", result);
        });

        group.MapPost("/login", async (LoginUserCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Ok(result);
        });

        return group;
    }
}
