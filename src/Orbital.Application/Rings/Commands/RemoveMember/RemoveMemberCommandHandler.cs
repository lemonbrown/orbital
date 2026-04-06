using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.RemoveMember;

public sealed class RemoveMemberCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<RemoveMemberCommand>
{
    public async Task Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ringId = RingId.From(request.RingId);
        var ring = await ringRepository.FindByIdWithDetailsAsync(ringId, cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var membershipId = MembershipId.From(request.MembershipId);
        var result = ring.RemoveMember(membershipId, currentUserService.UserId!.Value);

        if (result.IsFailure)
            throw new DomainException(result.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
