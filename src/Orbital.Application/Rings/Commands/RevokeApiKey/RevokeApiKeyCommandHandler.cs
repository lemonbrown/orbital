using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.RevokeApiKey;

public sealed class RevokeApiKeyCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<RevokeApiKeyCommand>
{
    public async Task Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ring = await ringRepository.FindByIdWithDetailsAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var result = ring.RevokeApiKey(RingApiKeyId.From(request.KeyId), currentUserService.UserId!.Value);
        if (result.IsFailure)
            throw new DomainException(result.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
