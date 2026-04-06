namespace Orbital.Application.Auth.Commands.LoginUser;

public sealed record LoginUserResponse(Guid UserId, string Username, string Email, string Token);
