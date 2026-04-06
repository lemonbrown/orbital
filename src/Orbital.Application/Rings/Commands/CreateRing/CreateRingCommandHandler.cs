using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.CreateRing;

public sealed class CreateRingCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateRingCommand, CreateRingResponse>
{
    public async Task<CreateRingResponse> Handle(CreateRingCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ownerId = currentUserService.UserId!.Value;

        var ringResult = Ring.Create(ownerId, request.Name, request.Description, request.Visibility);
        if (ringResult.IsFailure)
            throw new DomainException(ringResult.Error!);

        var ring = ringResult.Value;
        await ringRepository.AddAsync(ring, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRingResponse(ring.Id.Value, ring.Name, ring.Slug, ring.Description);
    }
}
