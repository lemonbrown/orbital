namespace Orbital.Domain.Sites;

public readonly record struct SiteId(Guid Value)
{
    public static SiteId New() => new(Guid.NewGuid());
    public static SiteId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
