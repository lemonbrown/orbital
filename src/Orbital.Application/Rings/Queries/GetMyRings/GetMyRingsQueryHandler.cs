using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetMyRings;

public sealed class GetMyRingsQueryHandler(
    IRingRepository ringRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMyRingsQuery, IReadOnlyList<RingListItemDto>>
{
    public async Task<IReadOnlyList<RingListItemDto>> Handle(GetMyRingsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var rings = await ringRepository.GetByOwnerAsync(currentUserService.UserId!.Value, cancellationToken);

        return rings.Select(r => new RingListItemDto(
            r.Id.Value,
            r.Name,
            r.Slug,
            r.Description,
            r.Visibility,
            r.Memberships.Count(m => m.Status == MembershipStatus.Approved),
            r.CreatedAt)).ToList();
    }
}
