using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.JoinRing;

public sealed record JoinRingResponse(Guid MembershipId, MembershipStatus Status);
