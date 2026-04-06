using MediatR;

namespace Orbital.Application.Rings.Commands.RemoveMember;

public sealed record RemoveMemberCommand(Guid RingId, Guid MembershipId) : IRequest;
