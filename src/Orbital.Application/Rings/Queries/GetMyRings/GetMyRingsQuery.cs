using MediatR;

namespace Orbital.Application.Rings.Queries.GetMyRings;

public sealed record GetMyRingsQuery : IRequest<IReadOnlyList<RingListItemDto>>;
