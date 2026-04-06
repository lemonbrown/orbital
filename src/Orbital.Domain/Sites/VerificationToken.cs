using Orbital.Domain.Common;

namespace Orbital.Domain.Sites;

public sealed class VerificationToken : ValueObject
{
    public string Value { get; }

    private VerificationToken(string value) => Value = value;

    public static VerificationToken Generate() =>
        new($"orbital-verify-{Guid.NewGuid():N}");

    public static VerificationToken FromString(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
