using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Users;

namespace Orbital.Application.Auth.Commands.LoginUser;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService)
    : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            throw new UnauthorizedException("Invalid credentials.");

        var user = await userRepository.FindByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null || !user.VerifyPassword(request.Password))
            throw new UnauthorizedException("Invalid credentials.");

        var token = jwtTokenService.GenerateToken(user);

        return new LoginUserResponse(user.Id.Value, user.Username, user.Email.Value, token);
    }
}
