namespace Orbital.Domain.Rings;

public readonly record struct RingApiKeyId(Guid Value)
{
    public static RingApiKeyId New() => new(Guid.NewGuid());
    public static RingApiKeyId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
