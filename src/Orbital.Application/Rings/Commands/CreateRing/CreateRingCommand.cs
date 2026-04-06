using MediatR;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.CreateRing;

public sealed record CreateRingCommand(
    string Name,
    string? Description,
    RingVisibility Visibility = RingVisibility.Public) : IRequest<CreateRingResponse>;
