using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.ApproveMember;

public sealed class ApproveMemberCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<ApproveMemberCommand, ApproveMemberResponse>
{
    public async Task<ApproveMemberResponse> Handle(ApproveMemberCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ringId = RingId.From(request.RingId);
        var ring = await ringRepository.FindByIdWithDetailsAsync(ringId, cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var membershipId = MembershipId.From(request.MembershipId);
        var result = ring.ApproveMember(membershipId, currentUserService.UserId!.Value);

        if (result.IsFailure)
            throw new DomainException(result.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var membership = ring.Memberships.First(m => m.Id == membershipId);
        return new ApproveMemberResponse(membership.Id.Value, membership.OrderIndex);
    }
}
