using Orbital.Domain.Common;

namespace Orbital.Domain.Rings;

public sealed class Dimension : ValueObject
{
    public static readonly Dimension Default = new("default");

    public string Value { get; }

    private Dimension(string value) => Value = value;

    public static Result<Dimension> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Dimension>("Dimension cannot be empty.");

        value = value.Trim().ToLowerInvariant();

        if (value.Length > 100)
            return Result.Failure<Dimension>("Dimension name cannot exceed 100 characters.");

        return Result.Success(new Dimension(value));
    }

    public static Dimension FromString(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
