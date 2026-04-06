using Orbital.Domain.Common;
using Orbital.Domain.Sites;

namespace Orbital.Domain.Rings.Events;

public sealed record MemberApprovedEvent(RingId RingId, SiteId SiteId, MembershipId MembershipId) : IDomainEvent;
