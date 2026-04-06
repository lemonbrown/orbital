using MediatR;
using Orbital.Application.Rings.Queries.GetMyRings;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetPublicRings;

public sealed class GetPublicRingsQueryHandler(IRingRepository ringRepository)
    : IRequestHandler<GetPublicRingsQuery, IReadOnlyList<RingListItemDto>>
{
    public async Task<IReadOnlyList<RingListItemDto>> Handle(GetPublicRingsQuery request, CancellationToken cancellationToken)
    {
        var rings = await ringRepository.GetPublicRingsAsync(cancellationToken);

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
