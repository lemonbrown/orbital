using MediatR;

namespace Orbital.Application.Rings.Queries.GetRingDirectory;

public sealed record GetRingDirectoryQuery(string Slug) : IRequest<IReadOnlyList<DirectorySiteDto>>;
