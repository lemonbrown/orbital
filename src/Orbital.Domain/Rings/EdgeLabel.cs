using Orbital.Domain.Common;

namespace Orbital.Domain.Rings;

public sealed class EdgeLabel : ValueObject
{
    public static readonly EdgeLabel Next = new("next");
    public static readonly EdgeLabel Previous = new("previous");

    public string Value { get; }

    private EdgeLabel(string value) => Value = value;

    public static EdgeLabel FromString(string value) => new(value.Trim().ToLowerInvariant());

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
