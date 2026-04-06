using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Queries.GetMyRings;

public sealed record RingListItemDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    RingVisibility Visibility,
    int MemberCount,
    DateTime CreatedAt);
