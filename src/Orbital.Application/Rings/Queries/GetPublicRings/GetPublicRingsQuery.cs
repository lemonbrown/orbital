using MediatR;
using Orbital.Application.Rings.Queries.GetMyRings;

namespace Orbital.Application.Rings.Queries.GetPublicRings;

public sealed record GetPublicRingsQuery : IRequest<IReadOnlyList<RingListItemDto>>;
