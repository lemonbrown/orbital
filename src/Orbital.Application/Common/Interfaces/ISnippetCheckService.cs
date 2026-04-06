namespace Orbital.Application.Common.Interfaces;

public interface ISnippetCheckService
{
    Task<bool> CheckAsync(string siteUrl, Guid siteId, CancellationToken ct = default);
}
