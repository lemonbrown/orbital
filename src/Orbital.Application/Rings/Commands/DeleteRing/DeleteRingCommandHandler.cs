using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.DeleteRing;

public sealed class DeleteRingCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteRingCommand>
{
    public async Task Handle(DeleteRingCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ringId = RingId.From(request.RingId);
        var ring = await ringRepository.FindByIdAsync(ringId, cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        if (ring.OwnerUserId != currentUserService.UserId!.Value)
            throw new UnauthorizedException("Only the ring owner can delete the ring.");

        await ringRepository.DeleteAsync(ring, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
