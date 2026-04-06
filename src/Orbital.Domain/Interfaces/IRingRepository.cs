using Orbital.Domain.Rings;
using Orbital.Domain.Users;

namespace Orbital.Domain.Interfaces;

public interface IRingRepository
{
    Task<Ring?> FindByIdAsync(RingId id, CancellationToken ct = default);
    Task<Ring?> FindByIdWithDetailsAsync(RingId id, CancellationToken ct = default);
    Task<Ring?> FindBySlugWithDetailsAsync(string slug, CancellationToken ct = default);
    Task<IReadOnlyList<Ring>> GetPublicRingsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Ring>> GetByOwnerAsync(UserId ownerId, CancellationToken ct = default);
    Task AddAsync(Ring ring, CancellationToken ct = default);
    Task DeleteAsync(Ring ring, CancellationToken ct = default);
}
