using System.Text.RegularExpressions;
using Orbital.Application.Common.Interfaces;

namespace Orbital.Infrastructure.Http;

public sealed partial class SnippetCheckService(IHttpClientFactory httpClientFactory) : ISnippetCheckService
{
    public async Task<bool> CheckAsync(string siteUrl, Guid siteId, CancellationToken ct = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient("verification");
            var html = await client.GetStringAsync(siteUrl, ct);

            // Look for <script ... data-id="{siteId}" ...> anywhere in the page
            var idPattern = $"""data-id=["']{Regex.Escape(siteId.ToString())}["']""";
            return ScriptTagRegex().IsMatch(html) &&
                   Regex.IsMatch(html, idPattern, RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex(@"<script\b[^>]*\bdata-id=[""'][^""']+[""'][^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptTagRegex();
}
