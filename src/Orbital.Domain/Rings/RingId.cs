namespace Orbital.Domain.Rings;

public readonly record struct RingId(Guid Value)
{
    public static RingId New() => new(Guid.NewGuid());
    public static RingId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
