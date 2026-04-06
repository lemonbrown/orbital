using Orbital.Domain.Common;
using Orbital.Domain.Users;

namespace Orbital.Domain.Rings.Events;

public sealed record RingCreatedEvent(RingId RingId, UserId OwnerId, string Name) : IDomainEvent;
