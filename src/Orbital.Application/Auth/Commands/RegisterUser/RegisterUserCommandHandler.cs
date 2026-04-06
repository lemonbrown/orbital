using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Users;

namespace Orbital.Application.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService)
    : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            throw new DomainException(emailResult.Error!);

        var existingByEmail = await userRepository.FindByEmailAsync(emailResult.Value, cancellationToken);
        if (existingByEmail is not null)
            throw new DomainException("A user with this email already exists.");

        var usernameTaken = await userRepository.ExistsByUsernameAsync(request.Username, cancellationToken);
        if (usernameTaken)
            throw new DomainException("Username is already taken.");

        var userResult = User.Create(request.Username, request.Email, request.Password);
        if (userResult.IsFailure)
            throw new DomainException(userResult.Error!);

        var user = userResult.Value;
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.GenerateToken(user);

        return new RegisterUserResponse(user.Id.Value, user.Username, user.Email.Value, token);
    }
}
