namespace Orbital.Application.Auth.Commands.RegisterUser;

public sealed record RegisterUserResponse(Guid UserId, string Username, string Email, string Token);
