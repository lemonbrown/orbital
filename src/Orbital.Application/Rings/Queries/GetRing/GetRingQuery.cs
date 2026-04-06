using MediatR;

namespace Orbital.Application.Rings.Queries.GetRing;

public sealed record GetRingQuery(Guid RingId) : IRequest<RingDetailDto>;
