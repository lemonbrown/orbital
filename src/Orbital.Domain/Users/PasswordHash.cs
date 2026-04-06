using Orbital.Domain.Common;

namespace Orbital.Domain.Users;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static PasswordHash Create(string plainTextPassword)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 12);
        return new PasswordHash(hash);
    }

    public static PasswordHash FromHash(string existingHash) => new(existingHash);

    public bool Verify(string plainTextPassword) =>
        BCrypt.Net.BCrypt.Verify(plainTextPassword, Value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
