using MediatR;

namespace Orbital.Application.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string Password) : IRequest<RegisterUserResponse>;
