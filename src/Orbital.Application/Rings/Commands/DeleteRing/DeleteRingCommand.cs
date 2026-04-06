using MediatR;

namespace Orbital.Application.Rings.Commands.DeleteRing;

public sealed record DeleteRingCommand(Guid RingId) : IRequest;
