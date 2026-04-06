using MediatR;

namespace Orbital.Application.Rings.Commands.JoinRing;

public sealed record JoinRingCommand(Guid RingId, Guid SiteId) : IRequest<JoinRingResponse>;
