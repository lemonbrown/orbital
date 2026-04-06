using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Domain.Interfaces;

public interface ISiteRepository
{
    Task<Site?> FindByIdAsync(SiteId id, CancellationToken ct = default);
    Task<IReadOnlyList<Site>> GetByOwnerAsync(UserId ownerId, CancellationToken ct = default);
    Task AddAsync(Site site, CancellationToken ct = default);
}
