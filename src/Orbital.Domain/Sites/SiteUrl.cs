using Orbital.Domain.Common;

namespace Orbital.Domain.Sites;

public sealed class SiteUrl : ValueObject
{
    public string Value { get; }

    private SiteUrl(string value) => Value = value;

    public static Result<SiteUrl> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<SiteUrl>("URL cannot be empty.");

        value = value.Trim();

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return Result.Failure<SiteUrl>("URL must be a valid HTTP or HTTPS address.");

        return Result.Success(new SiteUrl(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
