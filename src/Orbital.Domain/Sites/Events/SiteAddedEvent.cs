using Orbital.Domain.Common;
using Orbital.Domain.Users;

namespace Orbital.Domain.Sites.Events;

public sealed record SiteAddedEvent(SiteId SiteId, UserId? OwnerId, string Url) : IDomainEvent;
