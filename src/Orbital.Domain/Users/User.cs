using Orbital.Domain.Common;
using Orbital.Domain.Users.Events;

namespace Orbital.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    public string Username { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PasswordHash PasswordHash { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static Result<User> Create(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 50)
            return Result.Failure<User>("Username must be between 3 and 50 characters.");

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error!);

        var user = new User
        {
            Id = UserId.New(),
            Username = username.Trim(),
            Email = emailResult.Value,
            PasswordHash = PasswordHash.Create(password),
            CreatedAt = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Username, user.Email.Value));

        return Result.Success(user);
    }

    public bool VerifyPassword(string plainTextPassword) => PasswordHash.Verify(plainTextPassword);
}
