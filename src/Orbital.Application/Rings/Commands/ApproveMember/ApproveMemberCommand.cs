using MediatR;

namespace Orbital.Application.Rings.Commands.ApproveMember;

public sealed record ApproveMemberCommand(Guid RingId, Guid MembershipId) : IRequest<ApproveMemberResponse>;
