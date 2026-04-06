using Orbital.Domain.Common;
using Orbital.Domain.Sites;

namespace Orbital.Domain.Rings.Events;

public sealed record MemberJoinedRingEvent(RingId RingId, SiteId SiteId) : IDomainEvent;
