using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.CreateApiKey;

public sealed class CreateApiKeyCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResponse>
{
    public async Task<CreateApiKeyResponse> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ring = await ringRepository.FindByIdWithDetailsAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var result = ring.CreateApiKey(currentUserService.UserId!.Value, request.Label);
        if (result.IsFailure)
            throw new DomainException(result.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var (entity, plainKey) = result.Value;
        return new CreateApiKeyResponse(entity.Id.Value, entity.Label, plainKey, entity.CreatedAt);
    }
}
