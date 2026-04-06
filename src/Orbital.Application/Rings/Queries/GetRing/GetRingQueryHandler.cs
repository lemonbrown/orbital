using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetRing;

public sealed class GetRingQueryHandler(IRingRepository ringRepository)
    : IRequestHandler<GetRingQuery, RingDetailDto>
{
    public async Task<RingDetailDto> Handle(GetRingQuery request, CancellationToken cancellationToken)
    {
        var ringId = RingId.From(request.RingId);
        var ring = await ringRepository.FindByIdWithDetailsAsync(ringId, cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        return new RingDetailDto(
            ring.Id.Value,
            ring.Name,
            ring.Slug,
            ring.Description,
            ring.Visibility,
            ring.OwnerUserId.Value,
            ring.CreatedAt,
            ring.Memberships.Select(m => new MembershipDto(
                m.Id.Value, m.SiteId.Value, m.Role, m.Status, m.OrderIndex, m.JoinedAt)).ToList(),
            ring.Edges.Select(e => new EdgeDto(
                e.Id.Value, e.FromSiteId.Value, e.ToSiteId.Value,
                e.Dimension.Value, e.Label.Value, e.Weight)).ToList());
    }
}
