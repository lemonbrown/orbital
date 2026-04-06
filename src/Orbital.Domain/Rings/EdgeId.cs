namespace Orbital.Domain.Rings;

public readonly record struct EdgeId(Guid Value)
{
    public static EdgeId New() => new(Guid.NewGuid());
    public static EdgeId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
