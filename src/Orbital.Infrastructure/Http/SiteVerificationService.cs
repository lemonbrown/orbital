using System.Text.RegularExpressions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Sites;

namespace Orbital.Infrastructure.Http;

public sealed partial class SiteVerificationService(IHttpClientFactory httpClientFactory) : ISiteVerificationService
{
    public async Task<bool> VerifyAsync(SiteUrl url, VerificationToken token, CancellationToken ct = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient("verification");
            var html = await client.GetStringAsync(url.Value, ct);

            // Look for <meta name="orbital-verify" content="{token}">
            var pattern = $"""<meta\s+name=["']orbital-verify["']\s+content=["']{Regex.Escape(token.Value)}["']""";
            return MetaTagRegex().IsMatch(html) &&
                   Regex.IsMatch(html, pattern, RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex("""<meta\s+name=["']orbital-verify["']""", RegexOptions.IgnoreCase)]
    private static partial Regex MetaTagRegex();
}
