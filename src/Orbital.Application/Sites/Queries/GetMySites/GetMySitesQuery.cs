using MediatR;

namespace Orbital.Application.Sites.Queries.GetMySites;

public sealed record GetMySitesQuery : IRequest<IReadOnlyList<SiteDto>>;
