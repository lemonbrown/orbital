using MediatR;

namespace Orbital.Application.Auth.Commands.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResponse>;
