namespace Orbital.Domain.Rings;

public readonly record struct MembershipId(Guid Value)
{
    public static MembershipId New() => new(Guid.NewGuid());
    public static MembershipId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
