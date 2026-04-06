using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Api.Services;

/// <summary>
/// Validates a raw API key string and returns the associated RingId, or null if invalid.
/// </summary>
public sealed class RingApiKeyAuthService(IRingApiKeyRepository apiKeyRepository)
{
    public async Task<RingId?> ValidateAsync(string? key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length < 16)
            return null;

        var prefix = key[..16];
        var apiKey = await apiKeyRepository.FindByPrefixAsync(prefix, ct);
        if (apiKey is null || !apiKey.Verify(key))
            return null;

        return apiKey.RingId;
    }
}
