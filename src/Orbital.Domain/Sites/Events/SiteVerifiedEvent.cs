using Orbital.Domain.Common;

namespace Orbital.Domain.Sites.Events;

public sealed record SiteVerifiedEvent(SiteId SiteId) : IDomainEvent;
