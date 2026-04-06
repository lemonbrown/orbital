using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetApiKeys;

public sealed class GetApiKeysQueryHandler(
    IRingRepository ringRepository,
    IRingApiKeyRepository apiKeyRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetApiKeysQuery, IReadOnlyList<ApiKeyDto>>
{
    public async Task<IReadOnlyList<ApiKeyDto>> Handle(GetApiKeysQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ring = await ringRepository.FindByIdAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        if (ring.OwnerUserId != currentUserService.UserId!.Value)
            throw new UnauthorizedException("Only the ring owner can view API keys.");

        var keys = await apiKeyRepository.GetByRingAsync(ring.Id, cancellationToken);

        return keys.Select(k => new ApiKeyDto(
            k.Id.Value,
            k.Label,
            k.KeyPrefix,
            k.CreatedAt,
            k.IsRevoked,
            k.RevokedAt)).ToList();
    }
}
