using Orbital.Domain.Common;

namespace Orbital.Domain.Users;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>("Email cannot be empty.");

        value = value.Trim().ToLowerInvariant();

        if (!value.Contains('@') || value.Length > 256)
            return Result.Failure<Email>("Email is not valid.");

        return Result.Success(new Email(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
