using Orbital.Domain.Sites;

namespace Orbital.Application.Common.Interfaces;

public interface ISiteVerificationService
{
    /// <summary>
    /// Fetches the site URL and checks for &lt;meta name="orbital-verify" content="{token}"&gt;
    /// </summary>
    Task<bool> VerifyAsync(SiteUrl url, VerificationToken token, CancellationToken ct = default);
}
