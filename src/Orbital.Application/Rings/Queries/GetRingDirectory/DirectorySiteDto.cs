using Orbital.Domain.Sites;

namespace Orbital.Application.Rings.Queries.GetRingDirectory;

public sealed record DirectorySiteDto(
    Guid SiteId,
    string Name,
    string Url,
    string Description,
    int OrderIndex);
