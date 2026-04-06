using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Application.Rings.Commands.JoinRing;

public sealed class JoinRingCommandHandler(
    IRingRepository ringRepository,
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<JoinRingCommand, JoinRingResponse>
{
    public async Task<JoinRingResponse> Handle(JoinRingCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var siteId = SiteId.From(request.SiteId);
        var site = await siteRepository.FindByIdAsync(siteId, cancellationToken)
            ?? throw NotFoundException.For("Site", request.SiteId);

        if (site.OwnerUserId != currentUserService.UserId!.Value)
            throw new UnauthorizedException("You do not own this site.");

        var ringId = RingId.From(request.RingId);
        var ring = await ringRepository.FindByIdWithDetailsAsync(ringId, cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var result = ring.RequestJoin(siteId);
        if (result.IsFailure)
            throw new DomainException(result.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var membership = ring.Memberships.Last(m => m.SiteId == siteId);
        return new JoinRingResponse(membership.Id.Value, membership.Status);
    }
}
