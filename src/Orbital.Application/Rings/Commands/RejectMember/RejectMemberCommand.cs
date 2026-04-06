using MediatR;

namespace Orbital.Application.Rings.Commands.RejectMember;

public sealed record RejectMemberCommand(Guid RingId, Guid MembershipId) : IRequest;
