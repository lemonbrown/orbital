using Orbital.Domain.Rings;

namespace Orbital.Domain.Interfaces;

public interface IRingApiKeyRepository
{
    Task<RingApiKey?> FindByPrefixAsync(string prefix, CancellationToken ct = default);
    Task<IReadOnlyList<RingApiKey>> GetByRingAsync(RingId ringId, CancellationToken ct = default);
}
